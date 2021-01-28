﻿using CocNET.Interfaces;
using CocNET.Types.Players;
using IniParser;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Native.Csharp.App.Bot.BaseData;
using Unity;

namespace Native.Csharp.App.Bot
{
    public class AdminAPI
    {

        public static void ChangeNewMemberName(string id, CqAddGroupRequestEventArgs e)
        {
            ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
            var player = players.GetPlayer(id);
            var newname = BaseData.Instance.THLevels[player.TownHallLevel] + "本-" + player.Name;
            Common.CqApi.SetGroupMemberNewCard(e.FromGroup, e.FromQQ, newname);
            Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "新人看群文件部落规则，违反任何一条都将会被机票！群昵称已自动改为" + newname);
            Common.CqApi.SendGroupMessage(e.FromGroup, id);
        }

        public static bool NewMember(string id, CqAddGroupRequestEventArgs e)
        {
            if (Directory.GetFiles("com.coc.groupadmin\\Blacklist").Any(x=> x.EndsWith(e.FromQQ.ToString())))
            {
                //在黑名单内，直接拒绝
                return false;
            }
            Common.CqApi.AddLoger(LogerLevel.Info_Receive, "部落冲突群管", "接受到新人申请");
            ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
            List<int> levels = new List<int>();
            int heroLvNeed = 0;
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
                    if(troop.Village == "home")
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
                                troopsLV = BaseData.GetTownhallTroopsLV(player.TownHallLevel);
                            }
                            if (troopsLV[troop.Name.Replace(" ", "_")] > troop.Level)
                            {
                                troopFull = false;
                                try
                                {
                                    sb.AppendLine(BaseData.Instance.config["兵种翻译"][troop.Name.Replace(" ", "_")] + " 还缺" + (troopsLV[troop.Name.Replace(" ", "_")] - troop.Level) + "级");
                                    levels.Add(troopsLV[troop.Name.Replace(" ", "_")] - troop.Level);
                                }
                                catch
                                {
                                    sb.AppendLine(troop.Name + " 还缺" + (troopsLV[troop.Name.Replace(" ", "_")] - troop.Level) + "级");
                                }
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
                            troopsLV = BaseData.GetTownhallTroopsLV(player.TownHallLevel);
                        }
                        if (troopsLV[spell.Name.Replace(" ", "_")] > spell.Level)
                        {
                            spellFull = false;
                            try
                            {
                                sb.AppendLine(BaseData.Instance.config["兵种翻译"][spell.Name.Replace(" ", "_")] + " 还缺" + (troopsLV[spell.Name.Replace(" ", "_")] - spell.Level) + "级");
                                levels.Add(troopsLV[spell.Name.Replace(" ", "_")] - spell.Level);
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
                if (player.Heroes.Count > 0)
                {
                    sb.AppendLine("英雄：");
                    foreach (var hero in player.Heroes)
                    {
                        if (hero.Village == "home")
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
                                    troopsLV = BaseData.GetTownhallTroopsLV(player.TownHallLevel);
                                }
                                if (troopsLV[hero.Name.Replace(" ", "_")] > hero.Level)
                                {
                                    heroFull = false;
                                    heroLvNeed += (troopsLV[hero.Name.Replace(" ", "_")] - hero.Level);
                                    try
                                    {
                                        sb.AppendLine(BaseData.Instance.config["兵种翻译"][hero.Name.Replace(" ", "_")] + " 还缺" + (troopsLV[hero.Name.Replace(" ", "_")] - hero.Level) + "级");
                                    }
                                    catch
                                    {
                                        sb.AppendLine(hero.Name + " 还缺" + (troopsLV[hero.Name.Replace(" ", "_")] - hero.Level) + "级");
                                    }
                                }
                            }
                        }
                    }
                    if (heroFull)
                    {
                        sb.AppendLine("已满级");
                    }
                }
                bool allow;
                if (levels.Count(x => x > 1) > 6 || heroLvNeed >= 40)
                {
                    sb.AppendLine("不批准！科技不足！");
                    allow = false;
                }
                else
                {
                    sb.AppendLine("批准！科技已足够！");
                    allow = true;
                }
                if (player.WarStars >= 500)
                {
                    sb.AppendLine("战星已超过500，强制批准！");
                    allow = true;
                }
                Common.CqApi.SendGroupMessage(e.FromGroup,  "新人审核资料如下：\n" + sb.ToString());
                return allow;
            }
            else
            {
                //申请是来骂人的
                foreach (var keyvalue in valuePairs(configType.禁止词句))
                {
                    if (id == keyvalue.Key)
                    {
                        if (int.TryParse(keyvalue.Value, out int ticks))
                        {
                            if (ticks > 0)
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }
    }
}
