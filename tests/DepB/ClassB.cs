using net.r_eg.Components;

namespace DepB
{
    public class ClassB
    {
        public static MsgArgs GetMsgArgs(string msg) => DepC.ClassC.GetMsgArgs(msg);

        public static void SendStatic(string msg) => DepC.ClassC.SendStatic(msg);

        public static void SendInstance(string msg) => DepC.ClassC.SendInstance(msg);
    }
}
