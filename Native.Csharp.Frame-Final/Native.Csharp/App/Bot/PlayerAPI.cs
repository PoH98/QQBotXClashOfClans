using CocNET;
using CocNET.Interfaces;
using CocNET.Types.Players;
using IniParser;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Linq;
using System.Text;

namespace Native.Csharp.App.Bot
{
    public class PlayerAPI
    {
        public static void GetPlayer(string cocid, CqGroupMessageEventArgs e)
        {
            try
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "处理中...");
                if (!BaseData.CheckIP())
                {
                    TokenApi.GetNewToken();
                }
                if (cocid == BaseData.Instance.config["部落冲突"][e.FromGroup.ToString()])
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "你当我傻？拿部落标签给我查玩家？草你马的");
                    return;
                }
                ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                var player = players.GetPlayer(cocid);
                if (player != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("大本营等级：" + player.TownHallLevel + "，名字：" + player.Name);
                    sb.AppendLine("进攻次数: " + player.AttackWins + "，防御次数: " + player.DefenseWins);
                    sb.AppendLine("兵力：");
                    foreach (var troop in player.Troops)
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
                    sb.AppendLine("药水：");
                    foreach (var spell in player.Spells)
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
                    sb.AppendLine("英雄：");
                    foreach (var hero in player.Heroes)
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
                    Common.CqApi.SendGroupMessage(e.FromGroup, "@发送者 您需要的玩家资料在下面：\n@PlayerAPI".Replace("@PlayerAPI", sb.ToString()).Replace("@发送者",  Common.CqApi.CqCode_At(e.FromQQ)));
                }
                else
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, "未知的部落冲突ID，无法搜索该玩家资料！");
                }
            }
            catch (Exception ex)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "请确保发送/PlayerAPI时是/PlayerAPI 玩家标签！错误资料：" + ex.ToString());
            }
        }

        public static void CheckMember(CqGroupMessageEventArgs e)
        {
            string id = e.Message.Replace("/审核 ", "").Replace(" ", "");
            Common.CqApi.SendGroupMessage(e.FromGroup, "处理中...");
            if (id == BaseData.Instance.config["部落冲突"][e.FromGroup.ToString()])
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "你当我傻？拿部落标签给我查玩家？草你马的");
                return;
            }
            ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
            Player player = players.GetPlayer(id);
            if (player != null)
            {
                StringBuilder sb = new StringBuilder();
                bool troopFull = true, spellFull = true, heroFull = true;
                var troopsLV = BaseData.GetTownhallTroopsLV(player.TownHallLevel);
                sb.AppendLine("大本营等级：" + player.TownHallLevel + "，名字：" + player.Name);
                sb.AppendLine("兵力：");
                foreach (var troop in player.Troops)
                {
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
                        sb.AppendLine(troop.Name + " Townhall.ini 设置有错误，已自动生成");
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
                                sb.AppendLine(BaseData.Instance.translation[troop.Name.Replace(" ", "_")] + " 还缺" + (troopsLV[troop.Name.Replace(" ", "_")] - troop.Level) + "级");
                            }
                            catch
                            {
                                sb.AppendLine(troop.Name + " 还缺" + (troopsLV[troop.Name.Replace(" ", "_")] - troop.Level) + "级");
                            }
                        }
                    }
                }
                if (troopFull)
                {
                    sb.AppendLine("已满级");
                }
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
                        sb.AppendLine(spell.Name + " Townhall.ini 设置有错误，已自动生成");
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
                                sb.AppendLine(BaseData.Instance.translation[spell.Name.Replace(" ", "_")] + " 还缺" + (troopsLV[spell.Name.Replace(" ", "_")] - spell.Level) + "级");
                            }
                            catch
                            {
                                sb.AppendLine(spell.Name + " 还缺" + (troopsLV[spell.Name.Replace(" ", "_")] - spell.Level) + "级");
                            }
                        }
                    }
                }
                if (spellFull)
                {
                    sb.AppendLine("已满级");
                }
                if(player.Heroes.Count > 0)
                {
                    sb.AppendLine("英雄：");
                    foreach (var hero in player.Heroes)
                    {
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
                            sb.AppendLine(hero.Name + " Townhall.ini 设置有错误，已自动生成");
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
                                heroFull = false;
                                try
                                {
                                    sb.AppendLine(BaseData.Instance.translation[hero.Name.Replace(" ", "_")] + " 还缺" + (troopsLV[hero.Name.Replace(" ", "_")] - hero.Level) + "级");
                                }
                                catch
                                {
                                    sb.AppendLine(hero.Name + " 还缺" + (troopsLV[hero.Name.Replace(" ", "_")] - hero.Level) + "级");
                                }
                            }
                        }
                    }
                    if (heroFull)
                    {
                        sb.AppendLine("已满级");
                    }
                }
                if (sb.ToString().Split('\n').Length > 13)
                {
                    sb.AppendLine("注意：严禁升本！否则将会被机票！");
                }
                Common.CqApi.SendGroupMessage(e.FromGroup,  Common.CqApi.CqCode_At(e.FromQQ) + " 您要的审核资料如下：\n" + sb.ToString());
            }
            else
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "未知的部落冲突ID，无法搜索该玩家资料！");
            }
        }
    }
}
