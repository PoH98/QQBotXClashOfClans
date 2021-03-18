using Mirai_CSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QQBotXClashOfClans_v2
{
    public class 绑定群:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message.StartsWith("/绑定群 #"))
            {
                if (chat.Sender.Permission != GroupPermission.Member)
                {
                    string clanID = chat.Message.Split(' ').Where(x => x.Contains("#")).Last();
                    BaseData.SetClanID(chat.FromGroup, clanID);
                    return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage("已绑定" + chat.FromGroup + "为部落ID" + clanID)};
                }
                else
                {
                    return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage("我丢你蕾姆，你没权限用这个功能！")};
                }
            }
            return await base.GetReply(chat);
        }
    }
}
