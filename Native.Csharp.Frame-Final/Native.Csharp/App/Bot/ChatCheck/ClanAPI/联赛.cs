using CocNET.Interfaces;
using CocNET.Types.Clans.LeagueWar;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Bot
{
    public class 联赛:ChatCheckChain
    {
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message == "/联赛")
            {
                ICocCoreClans war = BaseData.Instance.container.Resolve<ICocCoreClans>();
                var keypairs = BaseData.valuePairs(configType.部落冲突);
                if (keypairs.ContainsKey(chat.FromGroup.ToString()))
                {
                    LeagueWar league = war.GetCurrentWarLeague(keypairs[chat.FromGroup.ToString()]);
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
                                    sb.AppendLine("拥有" + BaseData.Instance.THLevels[x] + "本 x" + count);
                            }
                            sb.AppendLine("============");

                        }
                        Common.CqApi.SendGroupMessage(chat.FromGroup, sb.ToString());
                        sb.Clear();
                        Array.Reverse(league.Rounds);
                        foreach (var rounds in league.Rounds)
                        {
                            foreach (var warTag in rounds.warTags)
                            {
                                if (warTag != "#0")
                                {
                                    var roundData = war.GetCurrentWarLeagueRound(warTag);
                                    Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "部落冲突", "联赛部落" + roundData.clan.Name + " vs " + roundData.opponent.Name);
                                    if (roundData.clan.Tag == keypairs[chat.FromGroup.ToString()].ToUpper())
                                    {
                                        if (roundData.state == "preparation")
                                        {
                                            sb.AppendLine("下场联赛开战时间为: " + roundData.StartTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                            sb.AppendLine("对手为" + roundData.opponent.Name);
                                            Common.CqApi.SendGroupMessage(chat.FromGroup, sb.ToString());
                                            sb.Clear();
                                        }
                                        else if (roundData.state == "inWar")
                                        {
                                            sb.AppendLine("当前联赛结束时间为: " + roundData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                            sb.AppendLine("对手为" + roundData.opponent.Name);
                                            sb.AppendLine("当前我方战星: " + roundData.clan.Stars + ", 敌方战星: " + roundData.opponent.Stars);
                                            return sb.ToString();
                                        }
                                    }
                                    else if (roundData.opponent.Tag == keypairs[chat.FromGroup.ToString()].ToUpper())
                                    {
                                        if (roundData.state == "preparation")
                                        {
                                            sb.AppendLine("下场联赛开战时间为: " + roundData.StartTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                            sb.AppendLine("对手为" + roundData.clan.Name);
                                            Common.CqApi.SendGroupMessage(chat.FromGroup, sb.ToString());
                                            sb.Clear();
                                        }
                                        else if (roundData.state == "inWar")
                                        {
                                            sb.AppendLine("当前联赛结束时间为: " + roundData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                            sb.AppendLine("对手为" + roundData.clan.Name);
                                            sb.AppendLine("当前我方战星: " + roundData.opponent.Stars + ", 敌方战星: " + roundData.clan.Stars);
                                            return sb.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(league.Reason))
                    {
                        if (league.Reason == "inMaintenance")
                        {
                            return " 当前服务器在维护！";
                        }
                    }
                    else
                    {
                        return "请在config.ini设置好Clan_ID后再继续使用此功能或者当前不在联赛时间";
                    }
                }
                else
                {
                    return "请在config.ini设置好Clan_ID后再继续使用此功能";
                }
            }
            return base.GetReply(chat);
        }
    }
}
