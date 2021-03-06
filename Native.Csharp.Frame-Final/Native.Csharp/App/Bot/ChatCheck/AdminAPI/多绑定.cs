using CocNET.Interfaces;
using Native.Csharp.App.Bot.Game;
using Native.Csharp.App.GameData;
using Native.Csharp.Sdk.Cqp;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace Native.Csharp.App.Bot.ChatCheck.AdminAPI
{
    public class 多绑定 : ChatCheckChain
    {
        public override IEnumerable<string> GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.StartsWith("/多绑定") && chat.Message.Where(x => x == '#').Count() > 1)
            {
                Common.CqApi.AddLoger(LogerLevel.Info_Receive, "部落冲突群管", "接受到改名指令");
                GroupMemberInfo sendMember = Common.CqApi.GetMemberInfo(chat.FromGroup, chat.FromQQ);
                string qq = "";
                List<string> ids = new List<string>();
                foreach (var cqCode in CqMsg.Parse(chat.Message).Contents)
                {
                    qq = cqCode.Dictionary["qq"];
                    break;
                }
                if (!long.TryParse(qq, out long tag))
                {
                    return new string[] { "我不知道你在艾特个毛线" };
                }
                if (chat.Message.Contains(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]))
                {
                    return new string[] { Common.CqApi.CqCode_At(chat.FromQQ) + "你当我傻？拿部落标签给我查玩家？草你马的" };
                }
                ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                if (tag == chat.FromQQ)
                {
                    ids.AddRange(chat.Message.Split(' ').Where(x => x.Contains("#")));
                    foreach (var id in ids)
                    {
                        if (!Member.ClanData.Any(x => x.ClanID == id.Trim().ToUpper()))
                        {
                            Member.ClanData.Add(new ClanData(id.Trim().ToUpper()));
                        }
                    }
                    string newname = string.Empty;
                    List<string> names = new List<string>();
                    foreach (var id in ids)
                    {
                        var player = players.GetPlayer(id.Trim());
                        if(player == null)
                        {
                            var remove = Member.ClanData.First(x => x.ClanID == id.Trim().ToUpper());
                            Member.ClanData.Remove(remove);
                        }
                        if (!string.IsNullOrEmpty(player.Reason))
                        {
                            var remove = Member.ClanData.First(x => x.ClanID == id.Trim().ToUpper());
                            Member.ClanData.Remove(remove);
                        }
                        var name = player.Name;
                        if (names.Contains(name.Substring(Math.Max(0, 3))) && name.StartsWithChinese())
                        {
                            //有重复名字
                            names.Add(name.Substring(Math.Max(0, name.Length - 3)));
                        }
                        else if (name.StartsWithChinese())
                        {
                            names.Add(string.Concat(name.Take(3)));
                        }
                        else
                        {
                            names.Add(name);
                        }
                    }

                    newname = string.Join(",", names);
                    Common.CqApi.SetGroupMemberNewCard(chat.FromGroup, tag, newname);
                    return new string[] { "搞定！已改称为" + newname };
                }
                else if (sendMember.PermitType == PermitType.Holder || sendMember.PermitType == PermitType.Manage)
                {
                    using (var API = new GameAPI(chat.FromGroup, tag))
                    {
                        var Member = API.Member;
                        ids.AddRange(chat.Message.Split(' ').Where(x => x.Contains("#")));
                        foreach (var id in ids)
                        {
                            if (!Member.ClanData.Any(x => x.ClanID == id.Trim().ToUpper()))
                            {
                                Member.ClanData.Add(new ClanData(id.Trim().ToUpper()));
                            }
                        }
                        string newname = string.Empty;
                        List<string> names = new List<string>();
                        foreach (var id in ids)
                        {
                            var player = players.GetPlayer(id.Trim());
                            if (player == null)
                            {
                                var remove = Member.ClanData.First(x => x.ClanID == id.Trim().ToUpper());
                                Member.ClanData.Remove(remove);
                            }
                            if (!string.IsNullOrEmpty(player.Reason))
                            {
                                var remove = Member.ClanData.First(x => x.ClanID == id.Trim().ToUpper());
                                Member.ClanData.Remove(remove);
                            }
                            var name = player.Name;
                            if (names.Contains(name.Substring(Math.Max(0, 3))) && name.StartsWithChinese())
                            {
                                //有重复名字
                                names.Add(name.Substring(Math.Max(0, name.Length - 3)));
                            }
                            else if (name.StartsWithChinese())
                            {
                                names.Add(string.Concat(name.Take(3)));
                            }
                            else
                            {
                                names.Add(name);
                            }
                        }

                        newname = string.Join(",", names);
                        Common.CqApi.SetGroupMemberNewCard(chat.FromGroup, tag, newname);
                        return new string[] { "搞定！已改称为" + newname };
                    }
                }
                else
                {
                    return new string[] { Common.CqApi.CqCode_At(chat.FromQQ) + "你没权限，别把我当脑残！" };
                }
            }
            return base.GetReply(chat);
        }
    }
}
