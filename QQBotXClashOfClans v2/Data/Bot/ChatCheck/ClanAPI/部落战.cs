using CocNET.Interfaces;
using Mirai_CSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace QQBotXClashOfClans_v2
{
    public class 部落战:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message == "/部落战")
            {
                ICocCoreClans clan = BaseData.Instance.container.Resolve<ICocCoreClans>();
                var clanData = clan.GetCurrentWar(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]);
                if (!string.IsNullOrEmpty(clanData.Reason))
                {
                    return new IMessageBase[]{new PlainMessage("无法获取部落资料！" + clanData.Reason)};
                }
                else
                {
                    if (clanData.State == "inWar")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("当前的部落战资料：\n");
                        sb.Append("当前我方战星: " + clanData.Clan.Stars + "(摧毁：" + clanData.Clan.DestructionPercentage + ")\n");
                        sb.Append("当前对手战星：" + clanData.Opponent.Stars + "(摧毁：" + clanData.Opponent.DestructionPercentage + ")\n");
                        sb.AppendLine();
                        foreach (var Member in clanData.Clan.Members.OrderBy(x => x.MapPosition))
                        {
                            if (Member.Attacks != null)
                            {
                                sb.Append(Member.Name + "已进攻 " + Member.Attacks.Count + " 次 \n");
                                int x = 1;
                                foreach (var attack in Member.Attacks)
                                {
                                    if (x == 1)
                                    {
                                        sb.Append("❶");
                                    }
                                    else
                                    {
                                        sb.Append("❷");
                                    }
                                    sb.Append("攻击了" + clanData.Opponent.Members.Where(y => y.Tag == attack.DefenderTag).FirstOrDefault().MapPosition + "号获得" + attack.Stars + "星|摧毁:" + attack.DestructionPercentage + "%\n");
                                    x++;
                                }
                                sb.Append("\n");
                            }
                            else
                            {
                                sb.Append(Member.Name + " " + Member.Tag + " 无进攻次数\n");
                            }
                        }
                        sb.Append("战斗日结束时间：" + clanData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                        return new IMessageBase[]{ BaseData.TextToImg(sb.ToString(),chat.Session) };
                    }
                    else if (clanData.State == "warEnded")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("当前部落战资料：\n");
                        sb.Append("当前我方战星: " + clanData.Clan.Stars + "(摧毁：" + clanData.Clan.DestructionPercentage + ")\n");
                        sb.Append("当前对手战星：" + clanData.Opponent.Stars + "(摧毁：" + clanData.Opponent.DestructionPercentage + ")\n");
                        sb.AppendLine();
                        foreach (var Member in clanData.Clan.Members.OrderBy(x => x.MapPosition))
                        {
                            if (Member.Attacks != null)
                            {
                                sb.Append(Member.Name + "已进攻 " + Member.Attacks.Count + " 次 \n");
                                int x = 1;
                                foreach (var attack in Member.Attacks)
                                {
                                    if (x == 1)
                                    {
                                        sb.Append("❶");
                                    }
                                    else
                                    {
                                        sb.Append("❷");
                                    }
                                    sb.Append("攻击了" + clanData.Opponent.Members.Where(y => y.Tag == attack.DefenderTag).FirstOrDefault().MapPosition + "号获得" + attack.Stars + "星|摧毁:" + attack.DestructionPercentage + "%\n");
                                    x++;
                                }
                                sb.Append("\n");
                            }
                            else
                            {
                                sb.Append(Member.Name + " 无进攻\n");
                            }
                        }
                        sb.Append("部落战已结束！");
                        return new IMessageBase[]{ BaseData.TextToImg(sb.ToString(),chat.Session) };
                    }
                    else if (clanData.State == "preparation")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("当前部落战资料：\n");
                        int x = 1;
                        foreach (var Member in clanData.Clan.Members.OrderBy(y => y.MapPosition))
                        {
                            sb.AppendLine(x + ". " + Member.Name);
                            x++;
                        }
                        sb.AppendLine("开战时间为: " + clanData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                        sb.AppendLine("当前为准备日");
                        return new IMessageBase[]{ BaseData.TextToImg(sb.ToString(),chat.Session) };
                    }
                    else if (clanData.Reason == "inMaintenance")
                    {
                       return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage(" 当前服务器在维护！")};
                    }
                    else
                    {
                        return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage(" 当前部落无部落战！")};
                    }
                }
            }
            return await base.GetReply(chat);
        }
    }
}
