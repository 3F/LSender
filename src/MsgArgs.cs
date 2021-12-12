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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace net.r_eg.Components
{
    [Serializable]
    public sealed class MsgArgs: EventArgs
    {
        public readonly DateTime stamp;

        public readonly string content;

        public readonly Exception exception;

        public readonly object data;

        public readonly MsgLevel level;

        public readonly IEnumerable<Vinf> vector;

        /// <summary>
        /// Is there a suitable assembly in the vector.
        /// </summary>
        /// <param name="name">Assembly name.</param>
        /// <returns>True if it is.</returns>
        public bool At(string name) => name == null ? false : vector.Any(v => v.name == name);

        /// <summary>
        /// Are there any suitable directions in the vector.
        /// </summary>
        /// <param name="map">Map of directions.</param>
        /// <returns>True if inside.</returns>
        public bool At(params string[] map)
        {
            if(map == null || map.Length < 1) {
                return false;
            }

            int idx = 0;
            foreach(var t in vector)
            {
                if(t.name != map[idx]) {
                    continue;
                }

                if(++idx == map.Length) {
                    return true;
                }
            }

            return false;
        }

        public MsgArgs(string msg, MsgLevel level = MsgLevel.Debug)
        {
            content     = msg;
            this.level  = level;
            stamp       = DateTime.Now;
            vector      = Track(2);
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

        private Vinf[] Track(int skip)
        {
            var rvector     = new Dictionary<string, Vinf>(12);
            string latest   = null;
            string current  = Assembly.GetExecutingAssembly().FullName;

            foreach(var frame in new StackTrace(skip, false).GetFrames())
            {
                MethodBase mb = frame.GetMethod();
                string asm = (mb.DeclaringType == null) ? mb.Module.Assembly.FullName // lambda, ~Anonymously Hosted DynamicMethods
                                                        : mb.DeclaringType.Assembly.FullName;

                if(asm != latest && asm != current && !rvector.ContainsKey(asm))
                {
                    var vinf = new Vinf(asm);
                    rvector[asm] = vinf;
                }

                latest = asm;
            }

            return rvector.Select(v => v.Value).ToArray();
        }
    }
}
