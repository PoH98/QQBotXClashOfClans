using CocNET.Interfaces;
using CocNET.Types.Clans.LeagueWar;
using Mirai_CSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace QQBotXClashOfClans_v2
{
    public class 联赛:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message == "/联赛")
            {
                ICocCoreClans war = BaseData.Instance.container.Resolve<ICocCoreClans>();
                var keypairs = BaseData.valuePairs(configType.部落冲突);
                if (keypairs.ContainsKey(chat.FromGroup.ToString()))
                {
                    List<IMessageBase> result = new List<IMessageBase>();
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
                        result.Add(BaseData.TextToImg(sb.ToString(),chat.Session));
                        sb.Clear();
                        Array.Reverse(league.Rounds);
                        foreach (var rounds in league.Rounds)
                        {
                            foreach (var warTag in rounds.warTags)
                            {
                                if (warTag != "#0")
                                {
                                    var roundData = war.GetCurrentWarLeagueRound(warTag);
                                    Logger.Instance.AddLog(LogType.Debug, "联赛部落" + roundData.clan.Name + " vs " + roundData.opponent.Name);
                                    if (roundData.clan.Tag == keypairs[chat.FromGroup.ToString()].ToUpper())
                                    {
                                        if (roundData.state == "preparation")
                                        {
                                            sb.AppendLine("下场联赛开战时间为: " + roundData.StartTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                            sb.AppendLine("对手为" + roundData.opponent.Name);
                                            result.Add(BaseData.TextToImg(sb.ToString(),chat.Session));
                                            sb.Clear();
                                        }
                                        else if (roundData.state == "inWar")
                                        {
                                            sb.AppendLine("当前联赛结束时间为: " + roundData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                            sb.AppendLine("对手为" + roundData.opponent.Name);
                                            sb.AppendLine("当前我方战星: " + roundData.clan.Stars + ", 敌方战星: " + roundData.opponent.Stars);
                                            result.Add(BaseData.TextToImg(sb.ToString(),chat.Session));
                                            break;
                                        }
                                    }
                                    else if (roundData.opponent.Tag == keypairs[chat.FromGroup.ToString()].ToUpper())
                                    {
                                        if (roundData.state == "preparation")
                                        {
                                            sb.AppendLine("下场联赛开战时间为: " + roundData.StartTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                            sb.AppendLine("对手为" + roundData.clan.Name);
                                            result.Add(BaseData.TextToImg(sb.ToString(),chat.Session));
                                            sb.Clear();
                                        }
                                        else if (roundData.state == "inWar")
                                        {
                                            sb.AppendLine("当前联赛结束时间为: " + roundData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                            sb.AppendLine("对手为" + roundData.clan.Name);
                                            sb.AppendLine("当前我方战星: " + roundData.opponent.Stars + ", 敌方战星: " + roundData.clan.Stars);
                                            result.Add(BaseData.TextToImg(sb.ToString(),chat.Session));
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        return result;
                    }
                    else if (!string.IsNullOrEmpty(league.Reason))
                    {
                        if (league.Reason == "inMaintenance")
                        {
                            return new IMessageBase[]{new PlainMessage(" 当前服务器在维护！")};
                        }
                    }
                    else
                    {
                        return new IMessageBase[]{new PlainMessage("请在config.ini设置好Clan_ID后再继续使用此功能或者当前不在联赛时间")};
                    }
                }
                else
                {
                    return new IMessageBase[]{new PlainMessage("请在config.ini设置好Clan_ID后再继续使用此功能")};
                }
            }
            return await base.GetReply(chat);
        }
    }
}
