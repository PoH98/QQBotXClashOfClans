using Mirai_CSharp;
using Mirai_CSharp.Models;

namespace QQBotXClashOfClans_v2.Data
{
    public class ApplyEventArgs
    {
        public IGroupApplyEventArgs EventArgs { get; set; }

        public MiraiHttpSession Session { get; set; }
    }
}
