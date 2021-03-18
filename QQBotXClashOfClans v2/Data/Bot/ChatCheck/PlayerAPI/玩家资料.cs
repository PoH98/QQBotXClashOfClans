using CocNET;
using CocNET.Interfaces;
using Mirai_CSharp.Models;
using QQBotXClashOfClans_v2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace QQBotXClashOfClans_v2.ChatCheck.PlayerAPI
{
    class 玩家资料:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message.ToLower().StartsWith("/玩家资料"))
            {
                try
                {
                    var cocid = chat.Message.Replace("/玩家资料", "").Trim();
                    if (!BaseData.CheckIP())
                    {
                        TokenApi.GetNewToken();
                    }
                    if (cocid == BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()])
                    {
                        return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage("你当我傻？拿部落标签给我查玩家？草你马的") };
                    }
                    ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                    var player = players.GetPlayer(cocid);
                    if (player != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("大本营等级：" + player.TownHallLevel + "，名字：" + player.Name);
                        sb.AppendLine("进攻次数: " + player.AttackWins + "，防御次数: " + player.DefenseWins + "，战星: " + player.WarStars);
                        sb.AppendLine("==================================");
                        sb.AppendLine("兵力：");
                        foreach (var troop in player.Troops)
                        {
                            try
                            {
                                sb.AppendLine("* " + BaseData.Instance.config["兵种翻译"][troop.Name.Replace(" ", "_")] + " - " + troop.Level + "级");
                            }
                            catch
                            {
                                sb.AppendLine("* " + troop.Name + " - " + troop.Level + "级");
                            }
                        }
                        sb.AppendLine("==================================");
                        sb.AppendLine("药水：");
                        foreach (var spell in player.Spells)
                        {
                            try
                            {
                                sb.AppendLine("* " + BaseData.Instance.config["兵种翻译"][spell.Name.Replace(" ", "_")] + " - " + spell.Level + "级");
                            }
                            catch
                            {
                                sb.AppendLine("* " + spell.Name + " - " + spell.Level + "级");
                            }
                        }
                        sb.AppendLine("==================================");
                        sb.AppendLine("英雄：");
                        foreach (var hero in player.Heroes)
                        {
                            try
                            {
                                sb.AppendLine("* " + BaseData.Instance.config["兵种翻译"][hero.Name.Replace(" ", "_")] + " - " + hero.Level + "级");
                            }
                            catch
                            {
                                sb.AppendLine("* " + hero.Name + " - " + hero.Level + "级");
                            }
                        }
                        return new IMessageBase[]{ BaseData.TextToImg(sb.ToString(),chat.Session) };
                    }
                    else
                    {
                        return new IMessageBase[]{new PlainMessage("未知的部落冲突ID，无法搜索该玩家资料！")};
                    }
                }
                catch (Exception ex)
                {
                    return new IMessageBase[]{new PlainMessage("请确保发送/PlayerAPI时是/PlayerAPI 玩家标签！错误资料：" + ex.Message)};
                }
            }
            return await base.GetReply(chat);
        }
    }
}
