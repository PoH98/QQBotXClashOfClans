using CocNET.Interfaces;
using CocNET.Types.Players;
using IniParser;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace QQBotXClashOfClans_v2.ChatCheck.PlayerAPI
{
    public class 审核:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message.StartsWith("/审核"))
            {
                string id;
                if (chat.Message.Contains("#"))
                {
                    //发送标签checkMember
                    id = chat.Message.Replace("/审核", "").Replace(" ", "");
                    if (id == BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()])
                    {
                        return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage( "你当我傻？拿部落标签给我查玩家？草你马的")};
                    }
                    return new IMessageBase[]{ new AtMessage(chat.FromQQ), CheckMember(id, chat.Session) };
                }
                else if (chat.Message == "/审核")
                {
                    if(Member.ClanData.Count < 1)
                    {
                        return new IMessageBase[]{ new PlainMessage("还请先使用 \"/绑定 #标签\" 指令后再使用不发玩家标签的审核指令！")};
                    }
                    List<IMessageBase> message = new List<IMessageBase>();
                    foreach(var clanData in Member.ClanData)
                    {
                        message.Add(CheckMember(clanData.ClanID, chat.Session));
                    }
                    return message;
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
                        return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage( "你当我傻？拿部落标签给我查玩家？草你马的")};
                    }
                    return new IMessageBase[]{ CheckMember(id, chat.Session) };
                }
                else
                {
                    return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage("无效的标签！")};
                }
            }
            return await base.GetReply(chat);
        }

        private static IMessageBase CheckMember(string id, MiraiHttpSession Session)
        {
            Logger.Instance.AddLog(LogType.Debug,"判断的部落ID为" + id);
            ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
            Player player = players.GetPlayer(id);
            if (player != null && string.IsNullOrEmpty(player.Reason))
            {
                StringBuilder sb = new StringBuilder();
                bool troopFull = true, spellFull = true, heroFull = true;
                int heroLvNeed = 0;
                var troopsLV = BaseData.GetTownhallTroopsLV(player.TownHallLevel);
                sb.AppendLine("大本营等级：" + player.TownHallLevel + "，名字：" + player.Name);
                sb.AppendLine("兵力：");
                foreach (var troop in player.Troops)
                {
                    if(troop.Village != "home")
                    {
                        continue;
                    }
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
                        sb.AppendLine("* "+troop.Name + " 还缺" + (troop.MaxLevel - troop.Level) + "级 (Townhall.ini设置丢失，自动生成)");
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
                            try
                            {
                                sb.AppendLine("* " + BaseData.Instance.config["兵种翻译"][troop.Name.Replace(" ", "_")] + " 还缺" + (troopsLV[troop.Name.Replace(" ", "_")] - troop.Level) + "级");
                            }
                            catch
                            {
                                sb.AppendLine("* " + troop.Name + " 还缺" + (troopsLV[troop.Name.Replace(" ", "_")] - troop.Level) + "级");
                            }
                        }
                    }
                }
                if (troopFull)
                {
                    sb.AppendLine("已满级");
                }
                sb.AppendLine("==================================");
                sb.AppendLine("药水：");
                foreach (var spell in player.Spells)
                {
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
                        sb.AppendLine("* " + spell.Name + " 还缺" + (spell.MaxLevel - spell.Level) + "级 (Townhall.ini设置丢失，自动生成)");
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
                            try
                            {
                                sb.AppendLine("* " + BaseData.Instance.config["兵种翻译"][spell.Name.Replace(" ", "_")] + " 还缺" + (troopsLV[spell.Name.Replace(" ", "_")] - spell.Level) + "级");
                            }
                            catch
                            {
                                sb.AppendLine("* " + spell.Name + " 还缺" + (troopsLV[spell.Name.Replace(" ", "_")] - spell.Level) + "级");
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
                    sb.AppendLine("==================================");
                    sb.AppendLine("英雄：");
                    foreach (var hero in player.Heroes)
                    {
                        if (hero.Village != "home")
                        {
                            continue;
                        }
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
                            sb.AppendLine("* " + hero.Name + " 还缺" + (hero.MaxLevel - hero.Level) + "级 (Townhall.ini设置丢失，自动生成)");
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
                                heroLvNeed += (troopsLV[hero.Name.Replace(" ", "_")] - hero.Level);
                                heroFull = false;
                                try
                                {
                                    sb.AppendLine("* " + BaseData.Instance.config["兵种翻译"][hero.Name.Replace(" ", "_")] + " 还缺" + (troopsLV[hero.Name.Replace(" ", "_")] - hero.Level) + "级");
                                }
                                catch
                                {
                                    sb.AppendLine("* " + hero.Name + " 还缺" + (troopsLV[hero.Name.Replace(" ", "_")] - hero.Level) + "级");
                                }
                            }
                        }
                    }
                    if (heroFull)
                    {
                        sb.AppendLine("已满级");
                    }
                }
                if (sb.ToString().Split('\n').Length > 13 || heroLvNeed >= 10)
                {
                    sb.AppendLine("==================================");
                    sb.AppendLine("注意：严禁升本！否则将会被机票！");
                }
                if (!Directory.Exists("Buffer"))
                {
                    Directory.CreateDirectory("Buffer");
                }
                return BaseData.TextToImg(sb.ToString(),Session);
            }
            else
            {
                if(player == null)
                {
                    return new PlainMessage("未知的部落冲突ID，无法搜索该玩家资料！");
                }
                else
                {
                    return new PlainMessage("出现错误，请稍后再试！错误详情：" + player.Reason);
                }

            }
        }
    }
}
