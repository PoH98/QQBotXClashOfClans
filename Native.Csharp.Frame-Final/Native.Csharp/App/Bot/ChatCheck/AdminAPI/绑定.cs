﻿using CocNET.Interfaces;
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

namespace Native.Csharp.App.Bot
{
    public class 改名:ChatCheckChain
    {
        public override IEnumerable<string> GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.StartsWith("/绑定"))
            {
                Common.CqApi.AddLoger(LogerLevel.Info_Receive, "部落冲突群管", "接受到改名指令");
                GroupMemberInfo sendMember = Common.CqApi.GetMemberInfo(chat.FromGroup, chat.FromQQ);
                string qq = "", newname;
                foreach (var cqCode in CqMsg.Parse(chat.Message).Contents)
                {
                    qq = cqCode.Dictionary["qq"];
                    break;
                }
                if (!long.TryParse(qq, out long tag))
                {
                    tag = chat.FromQQ;
                }
                if (chat.Message.Contains(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]))
                {
                    return new string[] { Common.CqApi.CqCode_At(chat.FromQQ) + "你当我傻？拿部落标签给我查玩家？草你马的" };
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
                            return new string[] { "找不到玩家资料或者玩家标签错误: " + player.Reason };
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
                                    if (names.Contains(name.Substring(Math.Min(name.Length, 3))) && name.StartsWithChinese())
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
                            return new string[] { "找不到玩家资料或者玩家标签错误: " + player.Reason };
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
                                    if (names.Contains(name.Substring(Math.Min(name.Length, 3))) && name.StartsWithChinese())
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
                            }
                            newname = string.Join(",", names);
                        }
                    }
                    else
                    {
                        return new string[] { "你给我的是什么鬼？！" };
                    }
                    Common.CqApi.SetGroupMemberNewCard(chat.FromGroup, tag, newname);
                    return  new string[] { "搞定！已改称为" + newname };
                }
                else if (sendMember.PermitType == PermitType.Holder || sendMember.PermitType == PermitType.Manage)
                {
                    using (var API = new GameAPI(chat.FromGroup, tag))
                    {
                        var Member = API.Member;
                        if (chat.Message.Contains('#') && chat.Message.Where(x => x == '#').Count() == 1)
                        {
                            newname = "#" + chat.Message.Split('#').Last().Trim().ToUpper();
                            ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                            var player = players.GetPlayer(newname);
                            if (!string.IsNullOrEmpty(player.Reason))
                            {
                                return new string[] { "找不到玩家资料或者玩家标签错误: " + player.Reason };
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
                                        if (names.Contains(name.Substring(Math.Min(name.Length, 3))) && name.StartsWithChinese())
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
                                return new string[] { "找不到玩家资料或者玩家标签错误: " + player.Reason };
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
                                        if (names.Contains(name.Substring(Math.Min(name.Length, 3))) && name.StartsWithChinese())
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
                                }
                                newname = string.Join(",", names);
                            }
                        }
                        else
                        {
                            throw new Exception();
                        }
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
