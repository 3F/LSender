using net.r_eg.Components;

namespace DepA
{
    public class ClassA
    {
        public static MsgArgs GetMsgArgs(string msg) => DepB.ClassB.GetMsgArgs(msg);

        public static void SendStatic(string msg) => DepB.ClassB.SendStatic(msg);

        public static void SendInstance(string msg) => DepB.ClassB.SendInstance(msg);
    }
}
