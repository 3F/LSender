using System;
using net.r_eg.Components;
using Xunit;

namespace LSenderTest
{
    public class LSenderTest
    {
        [Fact]
        public void SendTest1()
        {
            LSender.Reset();
            string content = "msg1";

            LSender.Sent += OnDefaultData1;
            LSender.Sent += (object sender, MsgArgs e) => 
            {
                Assert.Equal(content, e.content);
                Assert.NotNull(e.data);
                Assert.Equal(MsgLevel.Fatal, e.level);
            };

            LSender.Send(this, new MsgArgs(content, 1, MsgLevel.Fatal));
        }

        [Fact]
        public void SendTest2()
        {
            LSender.Reset();
            string content = "msg2";

            LSender.Sent += OnDefaultData3;
            LSender.Sent += (object sender, MsgArgs e) 
                => Assert.Equal(content, e.content);

            LSender.Send(this, content);
        }

        [Fact]
        public void SendTest3()
        {
            LSender.Reset();
            string content = "msg3";

            LSender.Sent += OnDefaultData2;
            LSender.Sent += (object sender, MsgArgs e) => 
            {
                Assert.Equal(content, e.content);
                Assert.Equal(MsgLevel.Error, e.level);
            };

            LSender.Send(this, content, MsgLevel.Error);
        }

        [Fact]
        public void SendTest4()
        {
            LSender.Reset();
            string content = "msg4";

            LSender.Sent += OnDefaultData3;
            LSender.Sent += (object sender, MsgArgs e) 
                => Assert.Equal(content, e.content);

            LSender.Send<LSenderTest>(new MsgArgs(content));
        }

        [Fact]
        public void SendTest5()
        {
            LSender.Reset();
            string content = "msg5";

            LSender.Sent += OnDefaultData3;
            LSender.Sent += (object sender, MsgArgs e) 
                => Assert.Equal(content, e.content);

            LSender.Send<LSenderTest>(content);
        }

        [Fact]
        public void SendTest6()
        {
            LSender.Reset();
            string content = "msg6";

            LSender.Sent += OnDefaultData2;
            LSender.Sent += (object sender, MsgArgs e) =>
            {
                Assert.Equal(content, e.content);
                Assert.Equal(MsgLevel.Trace, e.level);
            };

            LSender.Send<LSenderTest>(content, MsgLevel.Trace);
        }

        [Fact]
        public void SendVectorTest1()
        {
            LSender.Reset();

            LSender.Sent += (object sender, MsgArgs e) =>
            {
                Assert.True(e.At("LSenderTest"));
                Assert.True(e.At("DepC", "LSenderTest"));
                Assert.True(e.At("DepC", "DepB", "LSenderTest"));
                Assert.True(e.At("DepC", "DepB", "DepA", "LSenderTest"));

#if LSR_FEATURE_S_VECTOR
                Assert.False(e.At("DepB", "DepC", "LSenderTest"));
                Assert.False(e.At("DepA", "DepB", "LSenderTest"));
#else
                Assert.True(e.At("DepB", "DepC", "LSenderTest"));
                Assert.True(e.At("DepA", "DepB", "LSenderTest"));
#endif

                Assert.True(e.At("DepB", "DepA", "LSenderTest"));
            };

            DepA.ClassA.SendStatic(string.Empty);
            DepA.ClassA.SendInstance(string.Empty);
        }

        [Fact]
        public void SendVectorTest2()
        {
            LSender.Reset();

            LSender.Sent += (object sender, MsgArgs e) =>
            {
                Assert.True(e.At("LSenderTest"));
                Assert.True(e.At("DepC", "LSenderTest"));
            };

            DepC.ClassC.SendStatic(string.Empty);
            DepC.ClassC.SendInstance(string.Empty);
        }

        [Fact]
        public void ResetTest1()
        {
            LSender.Sent += (object sender, MsgArgs e) =>
            {
                Assert.True(false);
            };

            LSender.Reset();
            LSender.Send(this, String.Empty);
        }

        [Fact]
        public void RaiseTest1()
        {
            LSender._.Revoke();
            string content = "msg1";

            LSender._.Raised += OnDefaultData1;
            LSender._.Raised += (object sender, MsgArgs e) => 
            {
                Assert.Equal(content, e.content);
                Assert.NotNull(e.data);
                Assert.Equal(MsgLevel.Fatal, e.level);
            };

            LSender._.Raise(this, new MsgArgs(content, 1, MsgLevel.Fatal));
        }

        [Fact]
        public void RaiseTest2()
        {
            LSender._.Revoke();
            string content = "msg2";

            LSender._.Raised += OnDefaultData3;
            LSender._.Raised += (object sender, MsgArgs e) 
                => Assert.Equal(content, e.content);

            LSender._.Raise(this, content);
        }

        [Fact]
        public void RaiseTest3()
        {
            LSender._.Revoke();
            string content = "msg3";

            LSender._.Raised += OnDefaultData2;
            LSender._.Raised += (object sender, MsgArgs e) => 
            {
                Assert.Equal(content, e.content);
                Assert.Equal(MsgLevel.Error, e.level);
            };

            LSender._.Raise(this, content, MsgLevel.Error);
        }

        [Fact]
        public void RaiseTest4()
        {
            LSender._.Revoke();
            string content = "msg4";

            LSender._.Raised += OnDefaultData3;
            LSender._.Raised += (object sender, MsgArgs e) 
                => Assert.Equal(content, e.content);

            LSender._.Raise<LSenderTest>(new MsgArgs(content));
        }

        [Fact]
        public void RaiseTest5()
        {
            LSender._.Revoke();
            string content = "msg5";

            LSender._.Raised += OnDefaultData3;
            LSender._.Raised += (object sender, MsgArgs e) 
                => Assert.Equal(content, e.content);

            LSender._.Raise<LSenderTest>(content);
        }

        [Fact]
        public void RaiseTest6()
        {
            LSender._.Revoke();
            string content = "msg6";

            LSender._.Raised += OnDefaultData2;
            LSender._.Raised += (object sender, MsgArgs e) =>
            {
                Assert.Equal(content, e.content);
                Assert.Equal(MsgLevel.Trace, e.level);
            };

            LSender._.Raise<LSenderTest>(content, MsgLevel.Trace);
        }

        [Fact]
        public void RevokeTest1()
        {
            LSender._.Raised += (object sender, MsgArgs e) =>
            {
                Assert.True(false);
            };

            LSender._.Revoke();
            LSender._.Raise(this, string.Empty);
        }

        [Fact]
        public void CtorTest1()
        {
            LSender.Reset();

            // valid if no exception

            LSender.Send(this, string.Empty);
            LSender._.Raise(this, string.Empty);
        }

        [Fact]
        public void SenderObjectTest1()
        {
            LSender.Reset();

            LSender.Sent += (object sender, MsgArgs e) 
                => Assert.Equal(typeof(LSenderTest), sender.GetType());

            LSender.Send(this, string.Empty);
        }

        [Fact]
        public void SenderObjectTest2()
        {
            LSender.Reset();

            LSender.Sent += (object sender, MsgArgs e) 
                => Assert.Equal(typeof(LSenderTest), sender);

            LSender.Send<LSenderTest>(string.Empty);
        }

        [Fact]
        public void IsInTest1()
        {
            LSender.Reset();

            LSender.Sent += (object sender, MsgArgs e) => 
            {
#if LSR_FEATURE_S_VECTOR

                Assert.NotEmpty(e.vector);
                Assert.True(e.At("LSenderTest"));
                Assert.False(e.At("vsSolutionBuildEvent"));

                Assert.False(e.At("LSenderTest", "vsSolutionBuildEvent"));
                Assert.False(e.At("vsSolutionBuildEvent", "LSenderTest"));

#else
                Assert.Empty(e.vector);
                Assert.True(e.At("LSenderTest"));
                Assert.True(e.At("vsSolutionBuildEvent"));

                Assert.True(e.At("LSenderTest", "vsSolutionBuildEvent"));
                Assert.True(e.At("vsSolutionBuildEvent", "LSenderTest"));
#endif
            };

            LSender.Send(this, new MsgArgs(""));
        }

        private void OnDefaultData1(object sender, MsgArgs e)
        {
            var btype = typeof(LSenderTest);
            Assert.True(btype == sender.GetType() || btype == (Type)sender);

            Assert.Null(e.exception);
            Assert.True(e.stamp <= DateTime.Now);
        }

        private void OnDefaultData2(object sender, MsgArgs e)
        {
            OnDefaultData1(sender, e);
            Assert.Null(e.data);
        }

        private void OnDefaultData3(object sender, MsgArgs e)
        {
            OnDefaultData2(sender, e);
            Assert.Equal(MsgLevel.Debug, e.level);
        }
    }
}
