using Native.Csharp.Sdk.Cqp;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Native.Csharp.App.Bot
{
    class 拉黑:ChatCheckChain
    {
        public override IEnumerable<string> GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.StartsWith("/拉黑"))
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
                    var Member = Common.CqApi.GetMemberInfo(chat.FromGroup, tag);
                    foreach(var group in Common.CqApi.GetGroupList())
                    {
                        if (Common.CqApi.GetMemberList(group.Id).Any(x => x.QQId == Member.QQId))
                        {
                            Common.CqApi.SetGroupMemberRemove(group.Id, Member.QQId);
                            Common.CqApi.SendGroupMessage(group.Id, "检测到已被拉黑的人存在群里，自动踢出群！");
                        }
                    }
                    Common.CqApi.SetGroupMemberRemove(chat.FromGroup, Member.QQId);
                    if (!Directory.Exists("com.coc.groupadmin\\Blacklist"))
                    {
                        Directory.CreateDirectory("com.coc.groupadmin\\Blacklist");
                    }
                    File.WriteAllText("com.coc.groupadmin\\Blacklist\\" + Member.QQId, "");
                    return new string[] { "已把" + Member.Nick + "|" + Member.Card + "踢出群聊！" };
                }
            }
            return base.GetReply(chat);
        }
    }
}
