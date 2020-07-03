using CocNET;
using CocNET.Interfaces;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Bot.ChatCheck.PlayerAPI
{
    class playerapi:ChatCheckChain
    {
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.ToLower().Contains("/playerapi #"))
            {
                try
                {
                    var cocid = chat.Message.Split(' ').Where(x => x.Contains("#")).Last().Replace(" ", "");
                    if (!BaseData.CheckIP())
                    {
                        TokenApi.GetNewToken();
                    }
                    if (cocid == BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()])
                    {
                        return Common.CqApi.CqCode_At(chat.FromQQ) + "你当我傻？拿部落标签给我查玩家？草你马的";
                    }
                    ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                    var player = players.GetPlayer(cocid);
                    if (player != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("大本营等级：" + player.TownHallLevel + "，名字：" + player.Name);
                        sb.AppendLine("进攻次数: " + player.AttackWins + "，防御次数: " + player.DefenseWins + "，战星: " + player.WarStars);
                        sb.AppendLine("兵力：");
                        foreach (var troop in player.Troops)
                        {
                            if (BaseData.Instance.texts != null)
                            {
                                sb.AppendLine(BaseData.Instance.texts.Rows.Where(x => x["EN"].ToString() == troop.Name).First()["CN"].ToString() + " - " + troop.Level + "级");
                            }
                            else
                            {
                                try
                                {
                                    sb.AppendLine(BaseData.Instance.translation[troop.Name.Replace(" ", "_")] + " - " + troop.Level + "级");
                                }
                                catch
                                {
                                    sb.AppendLine(troop.Name + " - " + troop.Level + "级");
                                }
                            }
                        }
                        sb.AppendLine("药水：");
                        foreach (var spell in player.Spells)
                        {
                            if (BaseData.Instance.texts != null)
                            {
                                sb.AppendLine(BaseData.Instance.texts.Rows.Where(x => x["EN"].ToString() == spell.Name).First()["CN"].ToString() + " - " + spell.Level + "级");
                            }
                            else
                            {
                                try
                                {
                                    sb.AppendLine(BaseData.Instance.translation[spell.Name.Replace(" ", "_")] + " - " + spell.Level + "级");
                                }
                                catch
                                {
                                    sb.AppendLine(spell.Name + " - " + spell.Level + "级");
                                }
                            }
                        }
                        sb.AppendLine("英雄：");
                        foreach (var hero in player.Heroes)
                        {
                            if (BaseData.Instance.texts != null)
                            {
                                sb.AppendLine(BaseData.Instance.texts.Rows.Where(x => x["EN"].ToString() == hero.Name).First()["CN"].ToString() + " - " + hero.Level + "级");
                            }
                            else
                            {
                                try
                                {
                                    sb.AppendLine(BaseData.Instance.translation[hero.Name.Replace(" ", "_")] + " - " + hero.Level + "级");
                                }
                                catch
                                {
                                    sb.AppendLine(hero.Name + " - " + hero.Level + "级");
                                }
                            }
                        }
                        return  "@发送者 您需要的玩家资料在下面：\n@PlayerAPI".Replace("@PlayerAPI", sb.ToString()).Replace("@发送者", Common.CqApi.CqCode_At(chat.FromQQ));
                    }
                    else
                    {
                        return "未知的部落冲突ID，无法搜索该玩家资料！";
                    }
                }
                catch (Exception ex)
                {
                    return "请确保发送/PlayerAPI时是/PlayerAPI 玩家标签！错误资料：" + ex.ToString();
                }
            }
            return base.GetReply(chat);
        }
    }
}
