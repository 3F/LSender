using net.r_eg.Components;

namespace DepA
{
    public class ClassA
    {
        public static MsgArgs GetMsgArgs(string msg) => DepB.ClassB.GetMsgArgs(msg);
    }
}
