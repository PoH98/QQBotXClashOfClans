using CocNET.Interfaces;
using CocNET.Types.Clans.LeagueWar;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Bot.ChatCheck.ClanAPI
{
    public class 当前联赛:ChatCheckChain
    {
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message == "/当前联赛")
            {
                ICocCoreClans war = BaseData.Instance.container.Resolve<ICocCoreClans>();
                var keypairs = BaseData.valuePairs(configType.部落冲突);
                if (keypairs.ContainsKey(chat.FromGroup.ToString()))
                {
                    LeagueWar league = war.GetCurrentWarLeague(keypairs[chat.FromGroup.ToString()]);
                    if (league != null && string.IsNullOrEmpty(league.Reason))
                    {
                        StringBuilder sb = new StringBuilder();
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
                                        if (roundData.state == "inWar")
                                        {
                                            sb.AppendLine("当前联赛结束时间为: " + roundData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                            sb.AppendLine("对手为" + roundData.opponent.Name);
                                            sb.AppendLine("当前我方战星: " + roundData.clan.Stars + ", 敌方战星: " + roundData.opponent.Stars);
                                            sb.AppendLine("========================");
                                            foreach (var member in roundData.clan.Members.OrderBy(x => x.MapPosition))
                                            {
                                                if(member.Attacks == null || member.Attacks.Length < 1)
                                                {
                                                    sb.AppendLine(member.Name + " : 还未进攻");
                                                }
                                                else
                                                {
                                                    sb.AppendLine(member.Name + " : " + member.Attacks[0].Stars +"星(摧毁: "+member.Attacks[0].DestructionPercentage+"%)");
                                                }
                                            }
                                            return sb.ToString();
                                        }
                                    }
                                    else if (roundData.opponent.Tag == keypairs[chat.FromGroup.ToString()].ToUpper())
                                    {
                                        if (roundData.state == "inWar")
                                        {
                                            sb.AppendLine("当前联赛结束时间为: " + roundData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                            sb.AppendLine("对手为" + roundData.clan.Name);
                                            sb.AppendLine("当前我方战星: " + roundData.opponent.Stars + ", 敌方战星: " + roundData.clan.Stars);
                                            foreach (var member in roundData.opponent.Members.OrderBy(x => x.MapPosition))
                                            {
                                                if (member.Attacks == null || member.Attacks.Length < 1)
                                                {
                                                    sb.AppendLine(member.Name + " : 还未进攻");
                                                }
                                                else
                                                {
                                                    sb.AppendLine(member.Name + " : " + member.Attacks[0].Stars + "星(摧毁: " + member.Attacks[0].DestructionPercentage + "%)");
                                                }
                                            }
                                            return sb.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if(league != null)
                        {
                            return "无法获取任何联赛资料，错误详情: "+league.Reason + "！";
                        }
                        else
                        {
                            return "无法获取任何联赛资料！";
                        }
                    }
                }
                else
                {
                    return "请设置好config.ini后才使用此功能，或者使用</绑定群 #部落标签>后才使用！";
                }
            }
            return base.GetReply(chat);
        }
    }
}
