using Mirai_CSharp;
using Mirai_CSharp.Models;

namespace QQBotXClashOfClans_v2
{
    public class ChainEventArgs
    {
        public string Message { get; set; }

        public IGroupMemberInfo Sender { get; set; }

        public MiraiHttpSession Session { get; set; }

        public IMessageBase[] MessageChain { get; set; }

        public long FromGroup { get { return Sender.Group.Id; } }

        public long FromQQ { get { return Sender.Id; } }
    }
}
