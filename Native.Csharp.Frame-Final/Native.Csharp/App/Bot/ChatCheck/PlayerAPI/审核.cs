using CocNET.Interfaces;
using CocNET.Types.Players;
using IniParser;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace Native.Csharp.App.Bot.ChatCheck.PlayerAPI
{
    public class 审核:ChatCheckChain
    {
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.StartsWith("/审核"))
            {
                string id = string.Empty;
                if (chat.Message.Contains("#"))
                {
                    //发送标签checkMember
                    id = chat.Message.Replace("/审核", "").Replace(" ", "");
                    if (id == BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()])
                    {
                        return Common.CqApi.CqCode_At(chat.FromQQ) + "你当我傻？拿部落标签给我查玩家？草你马的";
                    }
                    return Common.CqApi.CqCode_At(chat.FromQQ) + checkMember(id);
                }
                else if (chat.Message == "/审核")
                {
                    var gameName = Common.CqApi.GetMemberInfo(chat.FromGroup, chat.FromQQ).Card;
                    if (gameName.Contains("-"))
                    {
                        ICocCoreClans cplayers = BaseData.Instance.container.Resolve<ICocCoreClans>();
                        var cplayer = cplayers.GetClansMembers(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]);
                        var member = cplayer.Where(x => x.Name == gameName.Split('-').Last()).FirstOrDefault();
                        if (member != null)
                        {
                            id = member.Tag;
                            return Common.CqApi.CqCode_At(chat.FromQQ) + checkMember(id);
                        }
                        else
                        {
                            return Common.CqApi.CqCode_At(chat.FromQQ) + "你不在部落里！请发送标签进行审核！";
                        }
                    }
                    else if (gameName.Contains(","))
                    {
                        ICocCoreClans cplayers = BaseData.Instance.container.Resolve<ICocCoreClans>();
                        var cplayer = cplayers.GetClansMembers(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]);
                        Random rnd = new Random();
                        foreach (var name in gameName.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                if (name.StartsWith(" "))
                                {
                                    var p = cplayer.Where(x => x.Name.Contains(name.Remove(0, name.LastIndexOf(' ') + 1))).FirstOrDefault();
                                    if (p != null)
                                    {
                                        Common.CqApi.SendGroupMessage(chat.FromGroup, Common.CqApi.CqCode_At(chat.FromQQ) + checkMember(p.Tag));
                                    }
                                    else
                                    {
                                        return Common.CqApi.CqCode_At(chat.FromQQ) + name + " 不在部落里！请发送标签进行审核！";
                                    }
                                }
                                else
                                {
                                    var p = cplayer.Where(x => x.Name.Contains(name)).FirstOrDefault();
                                    if (p != null)
                                    {
                                        Common.CqApi.SendGroupMessage(chat.FromGroup, Common.CqApi.CqCode_At(chat.FromQQ) + checkMember(p.Tag));
                                    }
                                    else
                                    {
                                        return Common.CqApi.CqCode_At(chat.FromQQ) + name + " 不在部落里！请发送标签进行审核！";
                                    }
                                }
                                //delay for a while
                                Thread.Sleep(rnd.Next(1000, 3000));
                            }
                        }
                        return string.Empty;
                    }
                    else if (gameName.Contains("，"))
                    {
                        ICocCoreClans cplayers = BaseData.Instance.container.Resolve<ICocCoreClans>();
                        var cplayer = cplayers.GetClansMembers(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]);
                        Random rnd = new Random();
                        foreach (var name in gameName.Split(','))
                        {
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                if (name.StartsWith(" "))
                                {
                                    var p = cplayer.Where(x => x.Name.Contains(name.Remove(0, name.LastIndexOf(' ') + 1))).FirstOrDefault();
                                    if (p != null)
                                    {
                                        Common.CqApi.SendGroupMessage(chat.FromGroup, Common.CqApi.CqCode_At(chat.FromQQ) + checkMember(p.Tag));
                                    }
                                    else
                                    {
                                        return Common.CqApi.CqCode_At(chat.FromQQ) + name + " 不在部落里！请发送标签进行审核！";
                                    }
                                }
                                else
                                {
                                    var p = cplayer.Where(x => x.Name.Contains(name)).FirstOrDefault();
                                    if (p != null)
                                    {
                                        Common.CqApi.SendGroupMessage(chat.FromGroup, Common.CqApi.CqCode_At(chat.FromQQ) + checkMember(p.Tag));
                                    }
                                    else
                                    {
                                        return Common.CqApi.CqCode_At(chat.FromQQ) + name + " 不在部落里！请发送标签进行审核！";
                                    }
                                }
                                //delay for a while
                                Thread.Sleep(rnd.Next(1000, 3000));
                            }
                        }
                        return string.Empty;
                    }
                    else
                    {
                        return Common.CqApi.CqCode_At(chat.FromQQ) + "无效的标签！";
                    }
                }
                else if (chat.Message.Contains("https") && chat.Message.Contains("tag="))
                {
                    //发送链接审核
                    id = chat.Message.Replace("/审核 ", "").Replace(" ", "");
                    id = "#" + id.Remove(0, id.LastIndexOf("tag=") + 4);
                    if (id.Contains("&"))
                    {
                        id = id.Remove(id.IndexOf('&'));
                    }
                    if (id == BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()])
                    {
                        return Common.CqApi.CqCode_At(chat.FromQQ) + "你当我傻？拿部落标签给我查玩家？草你马的";
                    }
                    return Common.CqApi.CqCode_At(chat.FromQQ) + checkMember(id);
                }
                else
                {
                    return Common.CqApi.CqCode_At(chat.FromQQ) + "无效的标签！";
                }
            }
            return base.GetReply(chat);
        }

        private static string checkMember(string id)
        {
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "部落Debug", "判断的部落ID为" + id);
            ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
            Player player = players.GetPlayer(id);
            if (player != null && string.IsNullOrEmpty(player.Reason))
            {
                StringBuilder sb = new StringBuilder();
                bool troopFull = true, spellFull = true, heroFull = true;
                var troopsLV = BaseData.GetTownhallTroopsLV(player.TownHallLevel);
                sb.AppendLine("大本营等级：" + player.TownHallLevel + "，名字：" + player.Name);
                sb.AppendLine("兵力：");
                foreach (var troop in player.Troops)
                {
                    Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "查询TID", "TID_" + troop.Name.Replace(" ", "_").ToUpper());
                    if (!troopsLV.Keys.Contains(troop.Name.Replace(" ", "_")))
                    {
                        for (int x = 1; x < BaseData.Instance.THLevels.Length; x++)
                        {
                            BaseData.Instance.thConfig[x.ToString() + "本"].AddKey(troop.Name.Replace(" ", "_"), troop.MaxLevel.ToString());
                        }
                        BaseData.Instance.config["兵种翻译"].AddKey(troop.Name.Replace(" ", "_"), troop.Name);
                        FileIniDataParser parser = new FileIniDataParser();
                        parser.WriteFile("Townhall.ini", BaseData.Instance.thConfig);
                        parser.WriteFile("config.ini", BaseData.Instance.config);
                        sb.AppendLine(troop.Name + " Townhall.ini 设置有错误，已自动生成");
                    }
                    else
                    {
                        if (troopsLV[troop.Name.Replace(" ", "_")] == 99)
                        {
                            BaseData.Instance.thConfig[player.TownHallLevel.ToString() + "本"][troop.Name.Replace(" ", "_")] = troop.MaxLevel.ToString();
                            FileIniDataParser parser = new FileIniDataParser();
                            parser.WriteFile("Townhall.ini", BaseData.Instance.thConfig);
                        }
                        if (troopsLV[troop.Name.Replace(" ", "_")] > troop.Level)
                        {
                            troopFull = false;
                            if (BaseData.Instance.texts != null)
                            {
                                sb.AppendLine(BaseData.Instance.texts.Rows.Where(x => x["EN"].ToString() == troop.Name).First()["CN"].ToString() + " 还缺" + (troopsLV[troop.Name.Replace(" ", "_")] - troop.Level) + "级");
                            }
                            else
                            {
                                try
                                {
                                    sb.AppendLine(BaseData.Instance.translation[troop.Name.Replace(" ", "_")] + " 还缺" + (troopsLV[troop.Name.Replace(" ", "_")] - troop.Level) + "级");
                                }
                                catch
                                {
                                    sb.AppendLine(troop.Name + " 还缺" + (troopsLV[troop.Name.Replace(" ", "_")] - troop.Level) + "级");
                                }
                            }
                        }
                    }
                }
                if (troopFull)
                {
                    sb.AppendLine("已满级");
                }
                sb.AppendLine("药水：");
                foreach (var spell in player.Spells)
                {
                    Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "查询TID", "TID_" + spell.Name.Replace(" ", "_").ToUpper());
                    if (!troopsLV.Keys.Contains(spell.Name.Replace(" ", "_")))
                    {
                        for (int x = 1; x < BaseData.Instance.THLevels.Length; x++)
                        {
                            BaseData.Instance.thConfig[x.ToString() + "本"].AddKey(spell.Name.Replace(" ", "_"), spell.MaxLevel.ToString());
                        }
                        BaseData.Instance.config["兵种翻译"].AddKey(spell.Name.Replace(" ", "_"), spell.Name);
                        FileIniDataParser parser = new FileIniDataParser();
                        parser.WriteFile("Townhall.ini", BaseData.Instance.thConfig);
                        parser.WriteFile("config.ini", BaseData.Instance.config);
                        sb.AppendLine(spell.Name + " Townhall.ini 设置有错误，已自动生成");
                    }
                    else
                    {
                        if (troopsLV[spell.Name.Replace(" ", "_")] == 99)
                        {
                            BaseData.Instance.thConfig[player.TownHallLevel + "本"][spell.Name.Replace(" ", "_")] = spell.MaxLevel.ToString();
                            FileIniDataParser parser = new FileIniDataParser();
                            parser.WriteFile("Townhall.ini", BaseData.Instance.thConfig);
                        }
                        if (troopsLV[spell.Name.Replace(" ", "_")] > spell.Level)
                        {
                            spellFull = false;
                            if (BaseData.Instance.texts != null)
                            {
                                sb.AppendLine(BaseData.Instance.texts.Rows.Where(x => x["EN"].ToString() == spell.Name).First()["CN"].ToString() + " 还缺" + (troopsLV[spell.Name.Replace(" ", "_")] - spell.Level) + "级");
                            }
                            else
                            {
                                try
                                {
                                    sb.AppendLine(BaseData.Instance.translation[spell.Name.Replace(" ", "_")] + " 还缺" + (troopsLV[spell.Name.Replace(" ", "_")] - spell.Level) + "级");
                                }
                                catch
                                {
                                    sb.AppendLine(spell.Name + " 还缺" + (troopsLV[spell.Name.Replace(" ", "_")] - spell.Level) + "级");
                                }
                            }
                        }
                    }
                }
                if (spellFull)
                {
                    sb.AppendLine("已满级");
                }
                if (player.Heroes.Count > 0)
                {
                    sb.AppendLine("英雄：");
                    foreach (var hero in player.Heroes)
                    {
                        Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "查询TID", "TID_" + hero.Name.Replace(" ", "_").ToUpper());
                        if (!troopsLV.Keys.Contains(hero.Name.Replace(" ", "_")))
                        {
                            for (int x = 1; x < BaseData.Instance.THLevels.Length; x++)
                            {
                                BaseData.Instance.thConfig[x.ToString() + "本"].AddKey(hero.Name.Replace(" ", "_"), hero.MaxLevel.ToString());
                            }
                            BaseData.Instance.config["兵种翻译"].AddKey(hero.Name.Replace(" ", "_"), hero.Name);
                            FileIniDataParser parser = new FileIniDataParser();
                            parser.WriteFile("Townhall.ini", BaseData.Instance.thConfig);
                            parser.WriteFile("config.ini", BaseData.Instance.config);
                            sb.AppendLine(hero.Name + " Townhall.ini 设置有错误，已自动生成");
                        }
                        else
                        {
                            if (troopsLV[hero.Name.Replace(" ", "_")] == 99)
                            {
                                BaseData.Instance.thConfig[player.TownHallLevel + "本"][hero.Name.Replace(" ", "_")] = hero.MaxLevel.ToString();
                                FileIniDataParser parser = new FileIniDataParser();
                                parser.WriteFile("Townhall.ini", BaseData.Instance.thConfig);
                            }
                            if (troopsLV[hero.Name.Replace(" ", "_")] > hero.Level)
                            {
                                heroFull = false;
                                if (BaseData.Instance.texts != null)
                                {
                                    sb.AppendLine(BaseData.Instance.texts.Rows.Where(x => x["EN"].ToString() == hero.Name).First()["CN"].ToString() + " 还缺" + (troopsLV[hero.Name.Replace(" ", "_")] - hero.Level) + "级");
                                }
                                else
                                {
                                    try
                                    {
                                        sb.AppendLine(BaseData.Instance.translation[hero.Name.Replace(" ", "_")] + " 还缺" + (troopsLV[hero.Name.Replace(" ", "_")] - hero.Level) + "级");
                                    }
                                    catch
                                    {
                                        sb.AppendLine(hero.Name + " 还缺" + (troopsLV[hero.Name.Replace(" ", "_")] - hero.Level) + "级");
                                    }
                                }
                            }
                        }
                    }
                    if (heroFull)
                    {
                        sb.AppendLine("已满级");
                    }
                }
                if (sb.ToString().Split('\n').Length > 13)
                {
                    sb.AppendLine("注意：严禁升本！否则将会被机票！");
                }
                return " 您要的审核资料如下：\n" + sb.ToString();
            }
            else
            {
                if(player == null)
                {
                    return "未知的部落冲突ID，无法搜索该玩家资料！";
                }
                else
                {
                    return "出现错误，请稍后再试！错误详情：" + player.Reason;
                }

            }
        }
    }
}
