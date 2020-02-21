﻿using CocNET.Interfaces;
using CocNET.Types.Clans.LeagueWar;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Linq;
using System.Text;
using static Native.Csharp.App.Bot.BaseData;

namespace Native.Csharp.App.Bot
{
    public class ClanAPI
    {
        /// <summary>
        /// 获取部落成员名单
        /// </summary>
        /// <param name="cocid"></param>
        /// <param name="e"></param>
        public static void GetClan(string cocid, CqGroupMessageEventArgs e)
        {
            try
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "处理中...");
                ICocCoreClans players = BaseData.Instance.container.Resolve<ICocCoreClans>();
                var player = players.GetClansMembers(cocid);
                if (player != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("部落成员数量：" + player.Count);
                    sb.AppendLine("成员列表: ");
                    foreach (var p in player)
                    {
                        sb.AppendLine(p.Name + " : " + p.Tag);
                    }
                    Common.CqApi.SendGroupMessage(e.FromGroup, "@发送者 您需要的玩家资料在下面：\n@PlayerAPI".Replace("@PlayerAPI", sb.ToString()).Replace("@发送者",  Common.CqApi.CqCode_At(e.FromQQ)));
                }
                else
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, "未知的部落冲突ID，无法搜索该玩家资料！");
                }

            }
            catch (Exception ex)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "请确保发送/ClanAPI时是/ClanAPI 玩家标签！错误资料：" + ex.ToString());
            }
        }
        /// <summary>
        /// 获取部落战资料
        /// </summary>
        /// <param name="e"></param>
        public static void GetWar(CqGroupMessageEventArgs e)
        {
            ICocCoreClans clan = BaseData.Instance.container.Resolve<ICocCoreClans>();
            var clanData = clan.GetCurrentWar(BaseData.Instance.config["部落冲突"][e.FromGroup.ToString()]);
            if (clanData == null)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "无法获取部落资料！");
            }
            else
            {
                if (clanData.State == "inWar")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append( Common.CqApi.CqCode_At(e.FromQQ) + "\n你要的部落战资料：\n");
                    sb.Append("当前我方战星: " + clanData.Clan.Stars + "(摧毁：" + clanData.Clan.DestructionPercentage + ")\n");
                    sb.Append("当前对手战星：" + clanData.Opponent.Stars + "(摧毁：" + clanData.Opponent.DestructionPercentage + ")\n");
                    sb.AppendLine();
                    foreach (var member in clanData.Clan.Members.OrderBy(x => x.MapPosition))
                    {
                        if (member.Attacks != null)
                        {
                            sb.Append(member.Name + "已进攻 " + member.Attacks.Count + " 次 \n");
                            int x = 1;
                            foreach (var attack in member.Attacks)
                            {
                                if (x == 1)
                                {
                                    sb.Append("❶");
                                }
                                else
                                {
                                    sb.Append("❷");
                                }
                                sb.Append(attack.Stars + "星|摧毁:" + attack.DestructionPercentage + "%\n");
                                x++;
                            }
                            sb.Append("\n");
                        }
                        else
                        {
                            sb.Append(member.Name + " " + member.Tag + " 无进攻次数\n");
                        }
                    }
                    sb.Append("战斗日结束时间：" + clanData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                    Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString());
                }
                else if (clanData.State == "warEnded")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append( Common.CqApi.CqCode_At(e.FromQQ) + "\n你要的部落战资料：\n");
                    sb.Append("当前我方战星: " + clanData.Clan.Stars + "(摧毁：" + clanData.Clan.DestructionPercentage + ")\n");
                    sb.Append("当前对手战星：" + clanData.Opponent.Stars + "(摧毁：" + clanData.Opponent.DestructionPercentage + ")\n");
                    sb.AppendLine();
                    foreach (var member in clanData.Clan.Members.OrderBy(x => x.MapPosition))
                    {
                        if (member.Attacks != null)
                        {
                            sb.Append(member.Name + "已进攻 " + member.Attacks.Count + " 次 \n");
                            int x = 1;
                            foreach (var attack in member.Attacks)
                            {
                                if (x == 1)
                                {
                                    sb.Append("❶");
                                }
                                else
                                {
                                    sb.Append("❷");
                                }
                                sb.Append(attack.Stars + "星|摧毁:" + attack.DestructionPercentage + "%\n");
                                x++;
                            }
                            sb.Append("\n");
                        }
                        else
                        {
                            sb.Append(member.Name + " 无进攻\n");
                        }
                    }
                    sb.Append("部落战已结束！");
                    Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString());
                }
                else if (clanData.State == "preparation")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(Common.CqApi.CqCode_At(e.FromQQ) + "\n你要的部落战资料：\n");
                    int x = 1;
                    foreach (var member in clanData.Clan.Members.OrderBy(y => y.MapPosition))
                    {
                        sb.AppendLine(x +". "+ member.Name);
                        x++;
                    }
                    sb.AppendLine("开战时间为: " + clanData.EndTime);
                    sb.AppendLine("当前为准备日");
                    Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString());
                }
                else if (clanData.Reason == "inMaintenance")
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + " 当前服务器在维护！");
                }
                else
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup,  Common.CqApi.CqCode_At(e.FromQQ) + " 当前部落无部落战！");
                }
            }
        }

        public static void GetLeagueWar(CqGroupMessageEventArgs e)
        {
            Common.CqApi.SendGroupMessage(e.FromGroup, "处理中...");
            ICocCoreClans war = Instance.container.Resolve<ICocCoreClans>();
            var keypairs = valuePairs(configType.部落冲突);
            if (keypairs.ContainsKey(e.FromGroup.ToString()))
            {
                LeagueWar league = war.GetCurrentWarLeague(keypairs[e.FromGroup.ToString()]);
                if (league != null && string.IsNullOrEmpty(league.Reason))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("============");
                    foreach (var clan in league.Clans)
                    {
                        sb.AppendLine("部落名: " + clan.Name);
                        sb.AppendLine("参赛成员：" + clan.Members.Length);
                        sb.AppendLine("-----------");
                        for (int x = 4; x <= 13; x++)
                        {
                            int count = clan.Members.Count(m => m.TownhallLevel == x);
                            if (count > 0)
                                sb.AppendLine("拥有" + Instance.THLevels[x] + "本 x" + count);
                        }
                        sb.AppendLine("============");

                    }
                    Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString());
                    sb.Clear();
                    Array.Reverse(league.Rounds);
                    bool ResourceGet = false;
                    foreach(var rounds in league.Rounds)
                    {
                        foreach(var warTag in rounds.warTags)
                        {
                            var roundData =  war.GetCurrentWarLeagueRound(warTag);
                            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "部落冲突", "联赛部落" + roundData.clan.Name +" vs "+ roundData.opponent.Name);
                            if(roundData.clan.Tag == keypairs[e.FromGroup.ToString()])
                            {
                                if(roundData.state == "preparation")
                                {
                                    sb.AppendLine("下场联赛开战时间为: " + roundData.StartTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                    int loc = 1;
                                    foreach (var member in roundData.clan.Members.ToList().OrderBy(x => x.MapPosition))
                                    {
                                        sb.AppendLine(loc + ". " +member.Name);
                                        loc++;
                                    }
                                    sb.AppendLine("对手为" + roundData.opponent.Name);
                                    Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString());
                                    sb.Clear();
                                }
                                else if (roundData.state == "inWar")
                                {
                                    sb.AppendLine("当前联赛结束时间为: " + roundData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                    int loc = 1;
                                    foreach (var member in roundData.clan.Members.ToList().OrderBy(x => x.MapPosition))
                                    {
                                        string attacked = "还没进攻";
                                        if (member.Attacks != null)
                                        {
                                            attacked = "已获得" + member.Attacks[0].Stars + "星！";
                                        }
                                        sb.AppendLine(loc + ". " +member.Name + " - " + attacked);
                                        loc++;
                                    }
                                    sb.AppendLine("对手为" + roundData.opponent.Name);
                                    Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString());
                                    sb.Clear();
                                    ResourceGet = true;
                                    break;
                                }
                            }
                            else if (roundData.opponent.Tag == keypairs[e.FromGroup.ToString()])
                            {
                                if (roundData.state == "preparation")
                                {
                                    sb.AppendLine("下场联赛开战时间为: " + roundData.StartTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                    int loc = 1;
                                    foreach (var member in roundData.opponent.Members.ToList().OrderBy(x => x.MapPosition))
                                    {
                                        sb.AppendLine(loc + ". " +member.Name);
                                        loc++;
                                    }
                                    sb.AppendLine("对手为" + roundData.clan.Name);
                                    Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString());
                                    sb.Clear();
                                }
                                else if (roundData.state == "inWar")
                                {
                                    sb.AppendLine("当前联赛结束时间为: " + roundData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                    int loc = 1;
                                    foreach (var member in roundData.opponent.Members.ToList().OrderBy(x => x.MapPosition))
                                    {
                                        string attacked = "还没进攻";
                                        if (member.Attacks != null)
                                        {
                                            attacked = "已获得" + member.Attacks[0].Stars + "星！";
                                        }
                                        sb.AppendLine(loc + ". " +member.Name + " - " + attacked);
                                        loc++;
                                    }
                                    sb.AppendLine("对手为" + roundData.clan.Name);
                                    Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString());
                                    sb.Clear();
                                    ResourceGet = true;
                                    break;
                                }
                            }
                        }
                        if (ResourceGet)
                        {
                            break;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(league.Reason))
                {
                    if (league.Reason == "inMaintenance")
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + " 当前服务器在维护！");
                    }
                }
                else
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, "请在config.ini设置好Clan_ID后再继续使用此功能或者当前不在联赛时间");
                }
            }
            else
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "请在config.ini设置好Clan_ID后再继续使用此功能");
            }
        }

        public static void GetMember(CqGroupMessageEventArgs e)
        {
            Common.CqApi.SendGroupMessage(e.FromGroup, "处理中...");
            ICocCoreClans players = BaseData.Instance.container.Resolve<ICocCoreClans>();
            var player = players.GetClansMembers(BaseData.Instance.config["部落冲突"][e.FromGroup.ToString()]);
            if (player != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("部落成员数量：" + player.Count);
                sb.AppendLine("成员列表: ");
                foreach (var p in player)
                {
                    sb.AppendLine(p.Name + " : " + p.Tag);
                }
                Common.CqApi.SendGroupMessage(e.FromGroup, "@发送者 您需要的玩家资料在下面：\n@PlayerAPI".Replace("@PlayerAPI", sb.ToString()).Replace("@发送者",  Common.CqApi.CqCode_At(e.FromQQ)));
            }
            else
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "未知的部落冲突ID，无法搜索该部落资料！");
            }
        }

        public static void GetWarLeft(CqGroupMessageEventArgs e)
        {
            ICocCoreClans clan = BaseData.Instance.container.Resolve<ICocCoreClans>();
            var clanData = clan.GetCurrentWar(BaseData.Instance.config["部落冲突"][e.FromGroup.ToString()]);
            if (clanData == null)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "无法获取部落资料！");
            }
            else
            {
                if (!string.IsNullOrEmpty(clanData.Reason))
                {
                    if (clanData.Reason == "inMaintenance")
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + " 当前服务器在维护！");
                    }
                }
                else if (clanData.State == "inWar")
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append( Common.CqApi.CqCode_At(e.FromQQ) + "\n你要的部落战资料：\n");
                    foreach (var member in clanData.Clan.Members)
                    {
                        if (member.Attacks == null)
                        {
                            sb.Append(member.Name + " " + member.Tag + "\n");
                        }
                    }
                    sb.Append("战斗日结束时间：" + clanData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                    Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString());
                }
                else
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup,  Common.CqApi.CqCode_At(e.FromQQ) + " 当前部落不在战斗日！(未开战或准备日)");
                }
            }
        }
    }
}
