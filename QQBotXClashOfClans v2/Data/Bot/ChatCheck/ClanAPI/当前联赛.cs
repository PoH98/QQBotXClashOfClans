using CocNET.Interfaces;
using CocNET.Types.Clans.LeagueWar;
using Mirai_CSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace QQBotXClashOfClans_v2.ChatCheck.ClanAPI
{
    public class 当前联赛:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
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
                                    Logger.Instance.AddLog(LogType.Debug, "联赛部落" + roundData.clan.Name + " vs " + roundData.opponent.Name);
                                    if (roundData.clan.Tag == keypairs[chat.FromGroup.ToString()].ToUpper())
                                    {
                                        if (roundData.state == "inWar")
                                        {
                                            sb.AppendLine("当前联赛结束时间为: " + roundData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                            sb.AppendLine("对手为" + roundData.opponent.Name);
                                            sb.AppendLine("当前我方战星: " + roundData.clan.Stars + ", 敌方战星: " + roundData.opponent.Stars);
                                            sb.AppendLine("========================");
                                            foreach (var Member in roundData.clan.Members.OrderBy(x => x.MapPosition))
                                            {
                                                if(Member.Attacks == null || Member.Attacks.Length < 1)
                                                {
                                                    sb.AppendLine(Member.Name + " : 还未进攻");
                                                }
                                                else
                                                {
                                                    sb.AppendLine(Member.Name + " : " + Member.Attacks[0].Stars +"星(摧毁: "+Member.Attacks[0].DestructionPercentage+"%)");
                                                }
                                            }
                                            return new IMessageBase[]{ BaseData.TextToImg(sb.ToString(),chat.Session) };
                                        }
                                    }
                                    else if (roundData.opponent.Tag == keypairs[chat.FromGroup.ToString()].ToUpper())
                                    {
                                        if (roundData.state == "inWar")
                                        {
                                            sb.AppendLine("当前联赛结束时间为: " + roundData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                                            sb.AppendLine("对手为" + roundData.clan.Name);
                                            sb.AppendLine("当前我方战星: " + roundData.opponent.Stars + ", 敌方战星: " + roundData.clan.Stars);
                                            foreach (var Member in roundData.opponent.Members.OrderBy(x => x.MapPosition))
                                            {
                                                if (Member.Attacks == null || Member.Attacks.Length < 1)
                                                {
                                                    sb.AppendLine(Member.Name + " : 还未进攻");
                                                }
                                                else
                                                {
                                                    sb.AppendLine(Member.Name + " : " + Member.Attacks[0].Stars + "星(摧毁: " + Member.Attacks[0].DestructionPercentage + "%)");
                                                }
                                            }
                                            return new IMessageBase[]{ BaseData.TextToImg(sb.ToString(),chat.Session) };
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
                            return new IMessageBase[]{new PlainMessage("无法获取任何联赛资料，错误详情: " + league.Reason + "！")};
                        }
                        else
                        {
                            return new IMessageBase[]{new PlainMessage("无法获取任何联赛资料！")};
                        }
                    }
                }
                else
                {
                    return new IMessageBase[]{new PlainMessage("请设置好config.ini后才使用此功能，或者使用</绑定群 #部落标签>后才使用！")};
                }
            }
            return await base.GetReply(chat);
        }
    }
}
