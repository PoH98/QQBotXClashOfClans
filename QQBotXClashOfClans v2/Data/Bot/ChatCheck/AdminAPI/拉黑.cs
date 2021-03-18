using Mirai_CSharp.Models;
using QQBotXClashOfClans_v2;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QQBotXClashOfClans_v2
{
    class 拉黑:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message.StartsWith("/拉黑"))
            {
                Logger.Instance.AddLog(LogType.Debug, "接受到踢人指令");
                var sender = chat.Sender;
                if (sender.Permission == GroupPermission.Member)
                {
                    return new IMessageBase[]{new PlainMessage("已把" + sender.Name + "踢出群聊！他娘的没权限还想踢人？")};
                }
                var qq = chat.MessageChain.Where(x => x is AtMessage);
                long tag;
                if(qq.Count() < 0)
                {
                    return new IMessageBase[] { new PlainMessage("我不知道你在艾特毛线！") };
                }
                tag = (qq.First() as AtMessage).Target;
                var member = await chat.Session.GetGroupMemberInfoAsync(tag, chat.FromGroup);
                Logger.Instance.AddLog(LogType.Debug, "已检测到QQ号" + qq);
                foreach (var group in await chat.Session.GetGroupListAsync())
                {
                    if ((await chat.Session.GetGroupMemberListAsync(group.Id)).Any(x => x.Id == tag))
                    {
                        await chat.Session.KickMemberAsync(tag, group.Id);
                        await chat.Session.SendGroupMessageAsync(group.Id, new PlainMessage("检测到已被拉黑的人存在群里，自动踢出群！"));
                    }
                }
                await chat.Session.KickMemberAsync(tag, chat.FromGroup);
                if (!Directory.Exists("com.coc.groupadmin\\Blacklist"))
                {
                    Directory.CreateDirectory("com.coc.groupadmin\\Blacklist");
                }
                File.WriteAllText("com.coc.groupadmin\\Blacklist\\" + tag, "");
                return new IMessageBase[] { new PlainMessage("已把" + member.Name + "踢出群聊！") };
            }
            return await base.GetReply(chat);
        }
    }
}
