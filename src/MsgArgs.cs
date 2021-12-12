/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2016-2021 Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) LSender contributors https://github.com/3F/LSender/graphs/contributors
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static net.r_eg.Components.Static.Polyfills;

#if LSR_FEATURE_S_VECTOR
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
#endif

namespace net.r_eg.Components
{
    [Serializable]
    public sealed class MsgArgs: EventArgs, ISerializable
    {
        public readonly DateTime stamp;

        public readonly string content;

        public readonly Exception exception;

        public readonly object data;

        public readonly MsgLevel level;

#if LSR_FEATURE_S_VECTOR
        public readonly IEnumerable<Vinf> vector;
        private static readonly Func<object> _GetStackFrames;
#else
        public readonly IEnumerable<Vinf> vector = EmptyArray<Vinf>();
#endif

        /// <summary>
        /// Is there a suitable assembly in the vector.
        /// </summary>
        /// <param name="name">Assembly name.</param>
        /// <returns>True if it is.</returns>
        public
#if LSR_USER_CODE && !LSR_FEATURE_S_VECTOR
#pragma warning disable CA1822 // Mark members as static
#endif
            bool At(string name)
#if LSR_USER_CODE && !LSR_FEATURE_S_VECTOR
#pragma warning restore CA1822
#endif
        {
#if LSR_FEATURE_S_VECTOR
            return name != null && vector.Any(v => v.name == name);
#else
            return true;
#endif
        }

        /// <summary>
        /// Are there any suitable directions in the vector.
        /// </summary>
        /// <param name="map">Map of directions.</param>
        /// <returns>True if inside.</returns>
        public
#if LSR_USER_CODE && !LSR_FEATURE_S_VECTOR
#pragma warning disable CA1822 // Mark members as static
#endif
            bool At(params string[] map)
#if LSR_USER_CODE && !LSR_FEATURE_S_VECTOR
#pragma warning restore CA1822
#endif
        {
#if LSR_FEATURE_S_VECTOR

            if(map == null || map.Length < 1) {
                return false;
            }

            int idx = 0;
            foreach(Vinf t in vector)
            {
                if(t.name != map[idx]) {
                    continue;
                }

                if(++idx == map.Length) {
                    return true;
                }
            }

            return false;

#else
            return true;
#endif
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if(info == null) throw new ArgumentNullException(nameof(info));

            info.AddValue(nameof(stamp), stamp);
            info.AddValue(nameof(content), content);
            info.AddValue(nameof(level), level);
            info.AddValue(nameof(exception), exception);
            info.AddValue(nameof(data), data);

#if LSR_FEATURE_S_VECTOR
            info.AddValue(nameof(vector), vector);
#endif
        }

#if LSR_FEATURE_S_VECTOR

        static MsgArgs()
        {
            _GetStackFrames = GenerateGetStackFrames();
        }

#endif

        public MsgArgs(string msg, MsgLevel level = MsgLevel.Debug)
        {
            content     = msg;
            this.level  = level;
            stamp       = DateTime.Now;

#if LSR_FEATURE_S_VECTOR
            vector = Track();
#endif
        }

        public MsgArgs(string msg, Exception ex, MsgLevel type = MsgLevel.Error)
            : this(msg, type)
        {
            exception = ex;
        }

        public MsgArgs(string msg, object data, MsgLevel type = MsgLevel.Debug)
            : this(msg, type)
        {
            this.data = data;
        }

        public MsgArgs(SerializationInfo info, StreamingContext context)
        {
            if(info == null) throw new ArgumentNullException(nameof(info));

            stamp   = info.GetDateTime(nameof(stamp));
            content = info.GetString(nameof(content));

            level       = (MsgLevel)info.GetValue(nameof(level), typeof(MsgLevel));
            exception   = (Exception)info.GetValue(nameof(exception), typeof(Exception));
            data        = info.GetValue(nameof(data), typeof(object));

#if LSR_FEATURE_S_VECTOR
            vector = new List<Vinf>
            (
                (IEnumerable<Vinf>)info.GetValue(nameof(vector), typeof(IEnumerable<Vinf>))
            );
#endif
        }

#if LSR_FEATURE_S_VECTOR

        private static ConstructorInfo IsStackFrameHelperValidCorlibV4(Type type)
        {
            if(type == null) return null;

            foreach(ConstructorInfo ctor in type.GetConstructors())
            {
                ParameterInfo[] args = ctor.GetParameters();
                if(args.Length == 1 && args[0].ParameterType == typeof(Thread))
                {
                    return ctor;
                }
            }
            return null;
        }

        private static Func<object> GenerateGetStackFrames()
        {
            // TODO: This implementation should be twice as fast as the original StackTrace instancing.
            //       But actually we don't need even the default StackFrameHelper logic because only the vector matters in our case.
            //       This however is not trivial task due to some CLR protections such as
            //       ECall methods must be packaged into a system module when calling GetStackFramesInternal which in turn requires StackFrameHelper instance.
            //       That's why we still wrap it inside StackFrameHelper class.
            //       Thus, In case of improving performance, reimplement all StackFrameHelper's virtual methods to nothing etc.

            Type tStackFrameHelper  = Type.GetType("System.Diagnostics.StackFrameHelper");
            ConstructorInfo ctor    = IsStackFrameHelperValidCorlibV4(tStackFrameHelper);

            if(ctor == null) return null;

            MethodInfo mGetStackFramesInternal = typeof(StackTrace).GetMethod
            (
                "GetStackFramesInternal", 
                BindingFlags.NonPublic | BindingFlags.Static
            );

            DynamicMethod m = new
            (
                nameof(_GetStackFrames), 
                typeof(object), 
                EmptyArray<Type>(), 
                typeof(StackTrace), 
                skipVisibility: true
            );

            ILGenerator il = m.GetILGenerator();
            il.DeclareLocal(tStackFrameHelper);

            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Stloc_0);

            // StackTrace.GetStackFramesInternal(this, iSkip, fNeedFileInfo, exception);
            il.Emit(OpCodes.Ldloc_0);

            // NOTE: I don't want `ldc.i4.s` because `1F + int8` vs `1A` (4) but you need update this if changing iSkip.
            //       Also note, too large values are dangerous due to incorrect results while too small values simply affect performance.
            il.Emit(OpCodes.Ldc_I4_4);  // iSkip = 4; delegate calls + .ctor -> Track() -> ( ... ) 
            il.Emit(OpCodes.Ldc_I4_0);  // fNeedFileInfo = false
            il.Emit(OpCodes.Ldnull);    // exception = null

            il.Emit(OpCodes.Call, mGetStackFramesInternal);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            return m.CreateDelegate(typeof(Func<object>)) as Func<object>;
        }

        private static IEnumerable<string> GetAsmFrames()
        {
            object frames = null;
            if(_GetStackFrames != null)
            {
                object o = _GetStackFrames();

                frames = o?.GetType().GetField
                (
                    "rgAssembly",
                    BindingFlags.NonPublic | BindingFlags.Instance
                )?
                .GetValue(o);
            }

            if(frames != null)
            {
                foreach(var frame in (object[])frames)
                {
                    yield return ((Assembly)frame).FullName;
                }
            }
            else
            {
                StackTrace strace = new(skipFrames: 3/*.ctor -> Track() -> ( ... )*/, fNeedFileInfo: false);
                foreach(StackFrame frame in strace.GetFrames())
                {
                    MethodBase mb = frame.GetMethod();
                    yield return mb?.DeclaringType?.Assembly.FullName
                                        ?? mb?.Module.Assembly.FullName; // lambda, ~Anonymously Hosted DynamicMethods
                }
            }
        }

        private static IEnumerable<Vinf> Track()
        {
            string latest   = null;
            string current  = Assembly.GetExecutingAssembly().FullName;

            Dictionary<string, Vinf> rvector = new(capacity: 20);
            foreach(string asm in GetAsmFrames())
            {
                if(asm != latest && asm != current && !rvector.ContainsKey(asm))
                {
                    rvector[asm] = new Vinf(asm);
                }

                latest = asm;
            }

            return rvector.Values;
        }

#endif
    }
}
