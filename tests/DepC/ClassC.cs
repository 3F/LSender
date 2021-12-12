using net.r_eg.Components;

namespace DepC
{
    public class ClassC
    {
        public static MsgArgs GetMsgArgs(string msg) => new MsgArgs(msg);

        public static void SendStatic(string msg) => LSender.Send<ClassC>(msg);

        public static void SendInstance(string msg) => LSender.Send(typeof(ClassC), msg);
    }
}
