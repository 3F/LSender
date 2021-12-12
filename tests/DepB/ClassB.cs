using net.r_eg.Components;

namespace DepB
{
    public class ClassB
    {
        public static MsgArgs GetMsgArgs(string msg) => DepC.ClassC.GetMsgArgs(msg);
    }
}
