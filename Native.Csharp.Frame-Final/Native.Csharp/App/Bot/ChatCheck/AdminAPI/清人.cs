using CocNET.Interfaces;
using Native.Csharp.App.GameData;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Native.Csharp.App.Bot
{
    public class 清人: ChatCheckChain
    {
        public override IEnumerable<string> GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.StartsWith("/清人"))
            {
                Common.CqApi.AddLoger(LogerLevel.Info_Receive, "部落冲突群管", "接受到检查指令");
                var Groupmember = Common.CqApi.GetMemberList(chat.FromGroup);
                ICocCoreClans players = BaseData.Instance.container.Resolve<ICocCoreClans>();
                var clanmembers = players.GetClansMembers(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]);
                if (clanmembers != null)
                {
                    var Clanmember = clanmembers.Select(x => x.Name).ToList();
                    if (Clanmember != null)
                    {
                        var namelist = getGroupMemberNameList(chat.FromGroup);
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("不在部落:");
                        foreach(var name in Clanmember)
                        {
                            var fetch = namelist.FirstOrDefault(x => name.Contains(x));
                            if (fetch == null)
                            {
                                sb.AppendLine(name);
                            }
                            else
                            {
                                namelist.Remove(fetch);
                            }
                        }
                        
                        return new string[] { BaseData.TextToImg("需要被清成员名单:\n" + sb.ToString()) };
                    }
                    else
                    {
                       return new string[] { "请确保config.ini里的设置是正确的！" };
                    }
                }
                else
                {
                   return new string[] { "请确保config.ini里的设置是正确的！" };
                }
            }
            return base.GetReply(chat);
        }

        public List<string> getGroupMemberNameList(long groupID)
        {
            var me = Common.CqApi.GetLoginQQ();
            List<string> namelist = new List<string>();
            foreach (var member in Common.CqApi.GetMemberList(groupID))
            {
                if (member.QQId != me)
                {
                    if (member.Card.Contains(","))
                    {
                        var splitted = member.Card.Split(',');
                        foreach (var split in splitted)
                        {
                            namelist.Add(split.Trim());
                        }
                    }
                    else if (member.Card.Contains("，"))
                    {
                        var splitted = member.Card.Split('，');
                        foreach (var split in splitted)
                        {
                            namelist.Add(split.Trim());
                        }
                    }
                    else if (member.Card.Contains("-"))
                    {
                        namelist.Add(string.Join("-", member.Card.Split('-').Skip(1)));
                    }
                    else if (string.IsNullOrEmpty(member.Card))
                    {
                        namelist.Add(member.Nick);
                    }
                    else
                    {
                        namelist.Add(member.Card);
                    }
                }

            }
            return namelist;
        }
    }
}
