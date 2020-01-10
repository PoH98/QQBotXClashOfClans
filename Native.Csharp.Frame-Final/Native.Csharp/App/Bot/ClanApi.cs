using CocNET.Interfaces;
using Native.Csharp.Sdk.Cqp;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Text;

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
            var clanData = clan.GetCurrentWar(BaseData.Instance.config["部落冲突"]["Clan_ID"]);
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
                    foreach (var member in clanData.Clan.Members)
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
                    foreach (var member in clanData.Clan.Members)
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
                else
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup,  Common.CqApi.CqCode_At(e.FromQQ) + " 当前部落无部落战！");
                }
            }
        }

        public static void GetMember(CqGroupMessageEventArgs e)
        {
            Common.CqApi.SendGroupMessage(e.FromGroup, "处理中...");
            ICocCoreClans players = BaseData.Instance.container.Resolve<ICocCoreClans>();
            var player = players.GetClansMembers(BaseData.Instance.config["部落冲突"]["Clan_ID"]);
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
            var clanData = clan.GetCurrentWar(BaseData.Instance.config["部落冲突"]["Clan_ID"]);
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
