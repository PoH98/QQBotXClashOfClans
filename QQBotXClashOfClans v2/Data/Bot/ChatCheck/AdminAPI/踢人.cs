using Mirai_CSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QQBotXClashOfClans_v2
{
    public class 踢人:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message.StartsWith("/踢"))
            {
                Logger.Instance.AddLog(LogType.Info, "接受到踢人指令");
                var sender = chat.Sender;
                if (sender.Permission == GroupPermission.Member)
                {
                    return new IMessageBase[]{new PlainMessage("已把" + sender.Name + "踢出群聊！他娘的没权限还想踢人？")};
                }
                var at = chat.MessageChain.Where(x => x is AtMessage);
                if (at.Count() < 1)
                {
                    Logger.Instance.AddLog(LogType.Info, "没有检测到QQ");
                    return new IMessageBase[] { };
                }
                var qq = (at.First() as AtMessage).Target;
                Logger.Instance.AddLog(LogType.Debug, "已检测到QQ号" + qq);
                var member = await chat.Session.GetGroupMemberInfoAsync(qq, chat.FromGroup);
                await chat.Session.KickMemberAsync(qq, chat.FromGroup);
                return new IMessageBase[] { new PlainMessage("已把" + member.Name + "踢出群聊！") };
            }
            return await base.GetReply(chat);
        }
    }
}
