using Mirai_CSharp.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QQBotXClashOfClans_v2.Data.Bot.ChatCheck.AdminAPI
{
    public class 绑定查看:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if(chat.Message == "/绑定查看")
            {
                StringBuilder sb = new StringBuilder();
                foreach (var data in Member.ClanData)
                {
                    sb.AppendLine(data.ClanID + ":" + data.Name);
                }
                return new IMessageBase[] { new PlainMessage(sb.ToString()) };
            }
            return await base.GetReply(chat);
        }
    }
}
