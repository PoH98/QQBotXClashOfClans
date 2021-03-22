using CocNET.Interfaces;
using Mirai_CSharp.Models;
using QQBotXClashOfClans_v2.Game;
using Native.Csharp.App.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace QQBotXClashOfClans_v2
{
    public class 改名:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message.StartsWith("/绑定"))
            {
                Logger.Instance.AddLog(LogType.Debug, "接受到改名指令");
                var sendMember = chat.Sender;
                string newname;
                var at = chat.MessageChain.Where(x => x is AtMessage);
                long tag = 0;
                if(at.Count() > 0)
                {
                    tag = (at.First() as AtMessage).Target;
                }
                else
                {
                    tag = chat.Sender.Id;
                }
                Logger.Instance.AddLog(LogType.Debug, "检测到艾特为" + tag);
                if (chat.Message.Contains(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]))
                {
                    return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage("你当我傻？拿部落标签给我查玩家？草你马的")};
                }
                if (tag == chat.FromQQ)
                {
                    if (chat.Message.Contains('#') && chat.Message.Where(x => x == '#').Count() == 1)
                    {
                        newname = "#" + chat.Message.Split('#').Last().Trim().ToUpper();
                        ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                        var player = players.GetPlayer(newname);
                        if (!string.IsNullOrEmpty(player.Reason))
                        {
                            return new IMessageBase[]{new PlainMessage("找不到玩家资料或者玩家标签错误: " + player.Reason)};
                        }
                        if (!Member.ClanData.Any(x => x.ClanID == newname))
                        {
                            Member.ClanData.Add(new ClanData(newname));
                        }
                        if (Member.ClanData.Count == 1)
                        {
                            newname = BaseData.Instance.THLevels[player.TownHallLevel] + "本-" + player.Name;
                        }
                        else
                        {
                            List<string> names = new List<string>();
                            foreach(var clanData in Member.ClanData)
                            {
                                var name = players.GetPlayer(clanData.ClanID).Name;
                                if(name.Length < 3)
                                {
                                    names.Add(name);
                                }
                                else
                                {
                                    if (names.Contains(name[Math.Min(name.Length, 3)..]) && name.StartsWithChinese())
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

                            }
                            newname = string.Join(",", names);
                        }
                    }
                    else if (chat.Message.Contains("http"))
                    {
                        newname = chat.Message.Replace("/审核", "").Replace(" ", "");
                        newname = "#" + newname.Remove(0, newname.LastIndexOf("tag=") + 4);
                        if (newname.Contains("&"))
                        {
                            newname = newname.Remove(newname.IndexOf('&'));
                        }
                        ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                        var player = players.GetPlayer(newname);
                        if (!string.IsNullOrEmpty(player.Reason))
                        {
                            return new IMessageBase[]{new PlainMessage("找不到玩家资料或者玩家标签错误: " + player.Reason)};
                        }
                        if (!Member.ClanData.Any(x => x.ClanID == newname))
                        {
                            Member.ClanData.Add(new ClanData(newname));
                        }
                        if (Member.ClanData.Count == 1)
                        {
                            newname = BaseData.Instance.THLevels[player.TownHallLevel] + "本-" + player.Name;
                        }
                        else
                        {
                            List<string> names = new List<string>();
                            foreach (var clanData in Member.ClanData)
                            {
                                var name = players.GetPlayer(clanData.ClanID).Name;
                                if (name.Length < 3)
                                {
                                    names.Add(name);
                                }
                                else
                                {
                                    if (names.Contains(name[Math.Min(name.Length, 3)..]) && name.StartsWithChinese())
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
                            }
                            newname = string.Join(",", names);
                        }
                    }
                    else
                    {
                        return new IMessageBase[]{new PlainMessage("你给我的是什么鬼？！")};
                    }
                    await chat.Session.ChangeGroupMemberInfoAsync(tag, chat.FromGroup, new GroupMemberCardInfo(newname , null));
                    return  new IMessageBase[]{new PlainMessage("搞定！已改称为" + newname)};
                }
                else if (sendMember.Permission != GroupPermission.Member)
                {
                    using var API = new GameAPI(chat.FromGroup, tag, chat.Session);
                    var Member = API.Member;
                    if (chat.Message.Contains('#') && chat.Message.Where(x => x == '#').Count() == 1)
                    {
                        newname = "#" + chat.Message.Split('#').Last().Trim().ToUpper();
                        ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                        var player = players.GetPlayer(newname);
                        if (!string.IsNullOrEmpty(player.Reason))
                        {
                            return new IMessageBase[] { new PlainMessage("找不到玩家资料或者玩家标签错误: " + player.Reason) };
                        }
                        if (!Member.ClanData.Any(x => x.ClanID == newname))
                        {
                            Member.ClanData.Add(new ClanData(newname));
                        }
                        if (Member.ClanData.Count == 1)
                        {
                            newname = BaseData.Instance.THLevels[player.TownHallLevel] + "本-" + player.Name;
                        }
                        else
                        {
                            List<string> names = new List<string>();
                            foreach (var clanData in Member.ClanData)
                            {
                                var name = players.GetPlayer(clanData.ClanID).Name;
                                if (name.Length < 3)
                                {
                                    names.Add(name);
                                }
                                else
                                {
                                    if (names.Contains(name[Math.Min(name.Length, 3)..]) && name.StartsWithChinese())
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
                            }
                            newname = string.Join(",", names);
                        }
                    }
                    else if (chat.Message.Contains("http"))
                    {
                        newname = chat.Message.Replace("/审核", "").Replace(" ", "");
                        newname = "#" + newname.Remove(0, newname.LastIndexOf("tag=") + 4);
                        if (newname.Contains("&"))
                        {
                            newname = newname.Remove(newname.IndexOf('&'));
                        }
                        ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                        var player = players.GetPlayer(newname);
                        if (!string.IsNullOrEmpty(player.Reason))
                        {
                            return new IMessageBase[] { new PlainMessage("找不到玩家资料或者玩家标签错误: " + player.Reason) };
                        }
                        if (!Member.ClanData.Any(x => x.ClanID == newname))
                        {
                            Member.ClanData.Add(new ClanData(newname));
                        }
                        if (Member.ClanData.Count == 1)
                        {
                            newname = BaseData.Instance.THLevels[player.TownHallLevel] + "本-" + player.Name;
                        }
                        else
                        {
                            List<string> names = new List<string>();
                            foreach (var clanData in Member.ClanData)
                            {
                                var name = players.GetPlayer(clanData.ClanID).Name;
                                if (name.Length < 3)
                                {
                                    names.Add(name);
                                }
                                else
                                {
                                    if (names.Contains(name[Math.Min(name.Length, 3)..]) && name.StartsWithChinese())
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
                            }
                            newname = string.Join(",", names);
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                    await chat.Session.ChangeGroupMemberInfoAsync(tag, chat.FromGroup, new GroupMemberCardInfo(newname, null));
                    return new IMessageBase[] { new PlainMessage("搞定！已改称为" + newname) };
                }
                else
                {
                    return new IMessageBase[]{ new AtMessage(chat.Sender.Id), new PlainMessage("你没权限，别把我当脑残！")};
                }
            }
            return await base.GetReply(chat);
        }
    }
}
