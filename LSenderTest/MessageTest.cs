using System;
using net.r_eg.Components;
using Xunit;

namespace LSenderTest
{
    public class MessageTest
    {
        [Fact]
        public void CtorTest1()
        {
            string content = "Message1";

            var msg = new MsgArgs(content, MsgLevel.Fatal);

            Assert.Equal(content, msg.content);
            Assert.Equal(MsgLevel.Fatal, msg.level);
            Assert.Null(msg.exception);
            Assert.Null(msg.data);
            Assert.True(msg.stamp <= DateTime.Now);
        }

        [Fact]
        public void CtorTest2()
        {
            string content = "Message2";

            try
            {
                throw new ArgumentNullException();
            }
            catch(ArgumentNullException ex)
            {
                var msg = new MsgArgs(content, ex, MsgLevel.Trace);

                Assert.Equal(content, msg.content);
                Assert.Equal(MsgLevel.Trace, msg.level);
                Assert.Null(msg.data);
                Assert.True(msg.stamp <= DateTime.Now);
                Assert.NotNull(msg.exception);

                Assert.Equal(typeof(ArgumentNullException), msg.exception.GetType());
            }
        }

        [Fact]
        public void CtorTest3()
        {
            string content = "Message3";

            object data = new int[] { 2, 4, 6 };

            var msg = new MsgArgs(content, data, MsgLevel.Warn);

            Assert.Equal(content, msg.content);
            Assert.Equal(MsgLevel.Warn, msg.level);
            Assert.True(msg.stamp <= DateTime.Now);
            Assert.Null(msg.exception);

            Assert.NotNull(msg.data);

            Assert.Equal(2, ((int[])msg.data)[0]);
            Assert.Equal(4, ((int[])msg.data)[1]);
            Assert.Equal(6, ((int[])msg.data)[2]);
        }

        [Fact]
        public void CtorTest4()
        {
            var msg = new MsgArgs(null);

            Assert.Null(msg.content);
            Assert.Equal(MsgLevel.Debug, msg.level);
            Assert.True(msg.stamp <= DateTime.Now);
            Assert.Null(msg.exception);
            Assert.Null(msg.data);
        }

        [Fact]
        public void CtorTest5()
        {
            var msg = new MsgArgs(null, "data");

            Assert.Null(msg.content);
            Assert.Equal(MsgLevel.Debug, msg.level);
            Assert.True(msg.stamp <= DateTime.Now);
            Assert.Null(msg.exception);
            Assert.NotNull(msg.data);
        }

        [Fact]
        public void CtorTest6()
        {
            var msg = new MsgArgs(null, new ArgumentOutOfRangeException());

            Assert.Null(msg.content);
            Assert.Equal(MsgLevel.Error, msg.level);
            Assert.True(msg.stamp <= DateTime.Now);
            Assert.Null(msg.data);
            Assert.NotNull(msg.exception);

            Assert.Equal(typeof(ArgumentOutOfRangeException), msg.exception.GetType());
        }

        [Fact]
        public void TrackTest1()
        {
            var msg = new MsgArgs("");

            Assert.True(msg.At("LSenderTest"));

            Assert.False(msg.At(string.Empty));
            Assert.False(msg.At(string.Empty, string.Empty));
            Assert.False(msg.At(" "));
            Assert.False(msg.At(new string[] { }));
            Assert.False(msg.At());
            Assert.False(msg.At((string[])null));
            Assert.False(msg.At((string)null));
        }

        [Fact]
        public void TrackTest2()
        {
            var msg = new MsgArgs("");

            Assert.True(msg.At("LSenderTest", "System.Threading.Thread"));
            Assert.True(msg.At("LSenderTest", "xunit.core"));
            Assert.True(msg.At("xunit.core", "System.Threading.Thread"));

            Assert.False(msg.At("System.Threading.Thread", "LSenderTest"));
            Assert.False(msg.At("xunit.core", "LSenderTest"));
            Assert.False(msg.At("System.Threading.Thread", "xunit.core"));
        }
    }
}
