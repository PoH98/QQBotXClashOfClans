using System;
using System.Collections.Generic;
using System.Linq;
using CocNET.Interfaces;
using QQBotXClashOfClans_v2.Game;
using System.Threading.Tasks;
using Unity;
using Mirai_CSharp.Models;

namespace QQBotXClashOfClans_v2.ChatCheck.AdminAPI
{
    public class 解绑 : ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message.StartsWith("/解绑"))
            {
                Logger.Instance.AddLog(LogType.Debug, "接受到改名指令");
                var sendMember = chat.Sender;
                string newname;
                var qq = chat.MessageChain.Where(x => x is AtMessage);
                long tag;
                if(qq.Count() < 1)
                {
                    tag = chat.FromQQ;
                }
                else
                {
                    tag = (qq.First() as AtMessage).Target;
                }
                if (chat.Message.Contains(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]))
                {
                    return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage("你当我傻？拿部落标签给我查玩家？草你马的")};
                }
                if (tag == chat.FromQQ)
                {
                    if (chat.Message.Contains('#'))
                    {
                        newname = chat.Message.Split(' ').Where(x => x.Contains("#")).Last().Trim().ToUpper();
                        if (Member.ClanData.Any(x => x.ClanID == newname))
                        {
                            var selected = Member.ClanData.Where(x => x.ClanID == newname).FirstOrDefault();
                            if(selected == null)
                            {
                                return new IMessageBase[]{new PlainMessage("找不到相关绑定，无法解绑！")};
                            }
                            Member.ClanData.Remove(selected);
                        }
                        else
                        {
                            return new IMessageBase[]{new PlainMessage("找不到相关绑定，无法解绑！")};
                        }
                        ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                        var player = players.GetPlayer(newname);
                        if (!string.IsNullOrEmpty(player.Reason))
                        {
                            return new IMessageBase[]{new PlainMessage("找不到玩家资料或者玩家标签错误: " + player.Reason)};
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
                        if (Member.ClanData.Any(x => x.ClanID == newname))
                        {
                            var selected = Member.ClanData.Where(x => x.ClanID == newname).FirstOrDefault();
                            if (selected == null)
                            {
                                return new IMessageBase[]{new PlainMessage("找不到相关绑定，无法解绑！")};
                            }
                            Member.ClanData.Remove(selected);
                        }
                        else
                        {
                            return new IMessageBase[]{new PlainMessage("找不到相关绑定，无法解绑！")};
                        }
                        ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                        var player = players.GetPlayer(newname);
                        if (!string.IsNullOrEmpty(player.Reason))
                        {
                            return new IMessageBase[]{new PlainMessage("找不到玩家资料或者玩家标签错误: " + player.Reason)};
                        }
                        else if (Member.ClanData.Count == 1)
                        {
                            newname = BaseData.Instance.THLevels[player.TownHallLevel] + "本-" + player.Name;
                        }
                        else
                        {
                            List<string> names = new List<string>();
                            foreach (var clanData in Member.ClanData)
                            {
                                var name = players.GetPlayer(clanData.ClanID).Name;
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
                        }
                    }
                    else
                    {
                        throw new Exception("???");
                    }
                    await chat.Session.ChangeGroupMemberInfoAsync(tag, chat.FromGroup, new GroupMemberCardInfo(newname, null));
                    return new IMessageBase[]{new PlainMessage("搞定！已改称为" + newname)};
                }
                else if (sendMember.Permission != GroupPermission.Member)
                {
                    using var API = new GameAPI(chat.FromGroup, tag, chat.Session);
                    var Member = API.Member;
                    if (chat.Message.Contains('#'))
                    {
                        newname = chat.Message.Split(' ').Where(x => x.Contains("#")).Last().Trim().ToUpper();
                        if (Member.ClanData.Any(x => x.ClanID == newname))
                        {
                            var selected = Member.ClanData.Where(x => x.ClanID == newname).FirstOrDefault();
                            if (selected == null)
                            {
                                return new IMessageBase[] { new PlainMessage("找不到相关绑定，无法解绑！") };
                            }
                            Member.ClanData.Remove(selected);
                        }
                        else
                        {
                            return new IMessageBase[] { new PlainMessage("找不到相关绑定，无法解绑！") };
                        }
                        ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                        var player = players.GetPlayer(newname);
                        if (!string.IsNullOrEmpty(player.Reason))
                        {
                            return new IMessageBase[] { new PlainMessage("找不到玩家资料或者玩家标签错误: " + player.Reason) };
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
                        if (Member.ClanData.Any(x => x.ClanID == newname))
                        {
                            var selected = Member.ClanData.Where(x => x.ClanID == newname).FirstOrDefault();
                            if (selected == null)
                            {
                                return new IMessageBase[] { new PlainMessage("找不到相关绑定，无法解绑！") };
                            }
                            Member.ClanData.Remove(selected);
                        }
                        else
                        {
                            return new IMessageBase[] { new PlainMessage("找不到相关绑定，无法解绑！") };
                        }
                        ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                        var player = players.GetPlayer(newname);
                        if (!string.IsNullOrEmpty(player.Reason))
                        {
                            return new IMessageBase[] { new PlainMessage("找不到玩家资料或者玩家标签错误: " + player.Reason) };
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
                    return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage("你没权限，别把我当脑残！")};
                }
            }
            return await base.GetReply(chat);
        }
    }
}
