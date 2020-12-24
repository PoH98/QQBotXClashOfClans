using Native.Csharp.Sdk.Cqp;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System.Collections.Generic;
using System.Linq;

namespace Native.Csharp.App.Bot
{
    public class 踢人:ChatCheckChain
    {
        public override IEnumerable<string> GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.StartsWith("/踢"))
            {
                Common.CqApi.AddLoger(LogerLevel.Info_Receive, "部落冲突群管", "接受到踢人指令");
                var sender = Common.CqApi.GetMemberInfo(chat.FromGroup, chat.FromQQ);
                if (sender.PermitType == PermitType.None)
                {
                    return new string[] { "已把" + sender.Card + "踢出群聊！他娘的没权限还想踢人？" };
                }
                string qq = "";
                var cqcontent = CqMsg.Parse(chat.Message).Contents;
                if (!cqcontent.Any(x => x.Dictionary.ContainsKey("qq")))
                {
                    Common.CqApi.AddLoger(LogerLevel.Info_Receive, "部落冲突群管", "没有检测到QQ");
                    return new string[] { };
                }
                foreach (var cqCode in cqcontent)
                {
                    qq = cqCode.Dictionary["qq"];
                    break;
                }
                Common.CqApi.AddLoger(LogerLevel.Debug, "部落冲突内测", "已检测到QQ号" + qq);
                if (!long.TryParse(qq, out long tag))
                {
                    return new string[] { };
                }
                else
                {
                    var member = Common.CqApi.GetMemberInfo(chat.FromGroup, tag);
                    Common.CqApi.SetGroupMemberRemove(chat.FromGroup, member.QQId);
                    return new string[] { "已把" + member.Nick + "|" + member.Card + "踢出群聊！" };
                }
            }
            return base.GetReply(chat);
        }
    }
}
