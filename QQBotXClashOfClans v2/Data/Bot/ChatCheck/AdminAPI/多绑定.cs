using CocNET.Interfaces;
using QQBotXClashOfClans_v2.Game;
using Native.Csharp.App.GameData;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Mirai_CSharp.Models;

namespace QQBotXClashOfClans_v2.ChatCheck.AdminAPI
{
    public class 多绑定 : ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message.StartsWith("/多绑定") && chat.Message.Where(x => x == '#').Count() > 1)
            {
                Logger.Instance.AddLog(LogType.Info, "接受到改名指令");
                var sendMember = chat.Sender;
                var ats = chat.MessageChain.Where(x => x is AtMessage);
                long tag;
                List<string> ids = new List<string>();
                if(ats.Count() < 1)
                {
                    tag = chat.FromQQ;
                }
                else
                {
                    tag = (ats.First() as AtMessage).Target;
                }
                if (chat.Message.Contains(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]))
                {
                    return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage("你当我傻？拿部落标签给我查玩家？草你马的")};
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
                        if (names.Contains(name[Math.Max(0, 3)..]) && name.StartsWithChinese())
                        {
                            //有重复名字
                            names.Add(name[Math.Max(0, name.Length - 3)..]);
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
                    await chat.Session.ChangeGroupMemberInfoAsync(tag, chat.FromGroup, new GroupMemberCardInfo(newname, null));
                    return new IMessageBase[]{new PlainMessage("搞定！已改称为" + newname)};
                }
                else if (sendMember.Permission != GroupPermission.Member)
                {
                    using var API = new GameAPI(chat.FromGroup, tag, chat.Session);
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
                        if (names.Contains(name[Math.Max(0, 3)..]) && name.StartsWithChinese())
                        {
                            //有重复名字
                            names.Add(name[Math.Max(0, name.Length - 3)..]);
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
                    await chat.Session.ChangeGroupMemberInfoAsync(tag, chat.FromGroup, new GroupMemberCardInfo(newname, null));
                    return new IMessageBase[] { new PlainMessage("搞定！已改称为" + newname) };
                }
                else
                {
                    return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage("你没权限，别把我当脑残！") };
                }
            }
            return await base.GetReply(chat);
        }
    }
}
