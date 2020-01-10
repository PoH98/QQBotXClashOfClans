using CocNET.Interfaces;
using CocNET.Types.Players;
using IniParser;
using Native.Csharp.Sdk.Cqp;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Native.Csharp.App.Bot
{
    public class AdminAPI
    {
        public static void CheckMember(CqGroupMessageEventArgs e)
        {
            Common.CqApi.SendGroupMessage(e.FromGroup, "处理中...");
            var Groupmember = Common.CqApi.GetMemberList(e.FromGroup);
            var me = Common.CqApi.GetLoginQQ();
            ICocCoreClans players = BaseData.Instance.container.Resolve<ICocCoreClans>();
            var Clanmember = players.GetClansMembers(BaseData.Instance.config["部落冲突"]["Clan_ID"]);
            if (Clanmember != null)
            {
                List<string> namelist = new List<string>();
                foreach (var member in Groupmember)
                {
                    if(member.QQId != me)
                    {
                        if (member.Card.Contains(","))
                        {
                            var splitted = member.Card.Split(',');
                            foreach (var split in splitted)
                            {
                                if (split.StartsWith(" "))
                                {
                                    namelist.Add(split.Remove(0, 1));
                                }
                            }
                        }
                        else if (member.Card.Contains("，"))
                        {
                            var splitted = member.Card.Split('，');
                            foreach (var split in splitted)
                            {
                                if (split.StartsWith(" "))
                                {
                                    namelist.Add(split.Remove(0, 1));
                                }
                            }
                        }
                        else if (member.Card.Contains("-"))
                        {
                            namelist.Add(member.Card.Split('-')[1]);
                        }
                        else
                        {
                            namelist.Add(member.Card);
                        }
                    }
                    
                }
                var reportMember = new List<string>();
                foreach (var mem in Clanmember)
                {
                    if (!namelist.Any(x => mem.Name.Contains(x)))
                    {
                        reportMember.Add("不在群：" + mem.Name);
                    }
                }
                foreach(var mem in namelist)
                {
                    if(!Clanmember.Any(x => x.Name.Contains(mem)))
                    {
                        reportMember.Add("不在部落：" + mem);
                    }
                }
                StringBuilder sb = new StringBuilder();
                foreach (var leftmember in reportMember)
                {
                    sb.AppendLine(leftmember);
                }
                Common.CqApi.SendGroupMessage(e.FromGroup, "需要被清成员名单:\n" + sb.ToString());
            }
            else
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "请确保config.ini里的设置是正确的！");
            }
        }

        public static void ChangeName(CqGroupMessageEventArgs e)
        {
            GroupMemberInfo sendMember = Common.CqApi.GetMemberInfo(e.FromGroup, e.FromQQ);
            if (sendMember.PermitType == PermitType.Holder || sendMember.PermitType == PermitType.Manage)
            {
                string qq = "", newname = e.Message.Split(' ').Where(x => x.Contains("#")).Last();
                foreach (var cqCode in CqMsg.Parse(e.Message).Contents)
                {
                    qq = cqCode.Dictionary["qq"];
                    break;
                }
                if (!long.TryParse(qq, out long tag))
                {
                    var list = Common.CqApi.GetMemberList(e.FromGroup);
                    foreach (var member in list)
                    {
                        if (member.Card == qq || member.Nick == qq)
                        {
                            tag = member.QQId;
                        }
                    }
                }
                if (newname.Contains('#'))
                {
                    ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                    var player = players.GetPlayer(newname);
                    newname = BaseData.Instance.THLevels[player.TownHallLevel] + "本-" + player.Name;
                }
                else
                {
                    throw new Exception();
                }
                Common.CqApi.SetGroupMemberNewCard(e.FromGroup, tag, newname);
                Common.CqApi.SendGroupMessage(e.FromGroup, "搞定！已改称为" + newname);
            }
            else
            {
                Common.CqApi.SendGroupMessage(e.FromGroup,  Common.CqApi.CqCode_At(e.FromQQ) + "你没权限，别把我当脑残！");
            }
        }

        public static bool NewMember(string id, CqAddGroupRequestEventArgs e)
        {
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
                if (player.Heroes.Count > 0)
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
                bool allow;
                if (sb.ToString().Count(x => Convert.ToInt32(x) > 1) > 3)
                {
                    sb.AppendLine("不批准！科技不足！");
                    allow = false;
                }
                else
                {
                    sb.AppendLine("批准！科技已足够！");
                    allow = true;
                }
                Common.CqApi.SendGroupMessage(e.FromGroup,  Common.CqApi.CqCode_At(e.FromQQ) + " 您要的审核资料如下：\n" + sb.ToString());
                return allow;
            }
            else
            {
                return true;
            }
        }
    }
}
