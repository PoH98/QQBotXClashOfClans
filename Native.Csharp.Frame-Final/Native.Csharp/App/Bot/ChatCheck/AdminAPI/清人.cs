using CocNET.Interfaces;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Native.Csharp.App.Bot
{
    public class 清人: ChatCheckChain
    {
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.StartsWith("/清人"))
            {
                Common.CqApi.AddLoger(LogerLevel.Info_Receive, "部落冲突群管", "接受到检查指令");
                Common.CqApi.SendGroupMessage(chat.FromGroup, "处理中...");
                var Groupmember = Common.CqApi.GetMemberList(chat.FromGroup);
                ICocCoreClans players = BaseData.Instance.container.Resolve<ICocCoreClans>();
                var clanmembers = players.GetClansMembers(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]);
                if (clanmembers != null)
                {
                    var Clanmember = clanmembers.Select(x => x.Name).ToList();
                    if (Clanmember != null)
                    {
                        var namelist = getGroupMemberNameList(chat.FromGroup);
                        var reportMember = new List<string>();
                        foreach (var mem in Clanmember)
                        {
                            //If not any group member name contains the clan member's name
                            if (!namelist.Any(x => mem.Contains(x)))
                            {
                                reportMember.Add("不在群：" + mem);
                            }
                        }
                        foreach (var mem in namelist)
                        {
                            if (!Clanmember.Any(x => x.Contains(mem)))
                            {
                                var _member = GameAPI.Instance.gameMembers[chat.FromGroup].Where(x => x.Member.Card.Contains(mem));
                                if (_member != null && _member.Count() > 0)
                                {
                                    var member = _member.First();
                                    if (member.LastSeenInClan == null)
                                    {
                                        member.LastSeenInClan = member.Member.JoiningTime;
                                    }
                                    reportMember.Add("不在部落：" + mem + "(最后记录在部落内" + Math.Round((DateTime.Now.Date - member.LastSeenInClan.Value).TotalDays).ToString("### ##0") + "天前)");
                                }
                            }
                        }
                        StringBuilder sb = new StringBuilder();
                        foreach (var leftmember in reportMember)
                        {
                            sb.AppendLine(leftmember);
                        }
                        return "需要被清成员名单:\n" + sb.ToString();
                    }
                    else
                    {
                       return "请确保config.ini里的设置是正确的！";
                    }
                }
                else
                {
                   return "请确保config.ini里的设置是正确的！";
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
                            if (split.StartsWith(" "))
                            {
                                namelist.Add(split.Remove(0, 1));
                            }
                            else
                            {
                                namelist.Add(split);
                            }
                        }
                    }
                    else if (member.Card.Contains("，"))
                    {
                        var splitted = member.Card.Split('，');
                        foreach (var split in splitted)
                        {
                            if (split.StartsWith(" "))
                            {
                                namelist.Add(split.Remove(0, 1));
                            }
                            else
                            {
                                namelist.Add(split);
                            }
                        }
                    }
                    else if (member.Card.Contains("-"))
                    {
                        namelist.Add(member.Card.Split('-')[1]);
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
