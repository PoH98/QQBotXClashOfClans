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
            Common.CqApi.AddLoger(LogerLevel.Info_Receive, "部落冲突群管", "接受到检查指令");
            Common.CqApi.SendGroupMessage(e.FromGroup, "处理中...");
            var Groupmember = Common.CqApi.GetMemberList(e.FromGroup);
            var me = Common.CqApi.GetLoginQQ();
            ICocCoreClans players = BaseData.Instance.container.Resolve<ICocCoreClans>();
            var clanmembers = players.GetClansMembers(BaseData.Instance.config["部落冲突"][e.FromGroup.ToString()]);
            if(clanmembers != null)
            {
                var Clanmember = clanmembers.Select(x => x.Name).ToList();
                if (Clanmember != null)
                {
                    List<string> namelist = new List<string>();
                    foreach (var member in Groupmember)
                    {
                        if (member.QQId != me)
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
                                    else
                                    {
                                        namelist.Add(split);
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
                                    else
                                    {
                                        namelist.Add(split);
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
                        //If not any group member name contains the clan member's name
                        if (!namelist.Any(x => mem.Contains(x)))
                        {
                            reportMember.Add("不在群：" + mem);
                        }
                    }
                    foreach (var mem in namelist)
                    {
                        if (!Clanmember.Any(x => x.Contains(mem)))
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
            else
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "请确保config.ini里的设置是正确的！");
            }
        }

        public static void ChangeName(CqGroupMessageEventArgs e)
        {
            Common.CqApi.AddLoger(LogerLevel.Info_Receive, "部落冲突群管", "接受到改名指令");
            GroupMemberInfo sendMember = Common.CqApi.GetMemberInfo(e.FromGroup, e.FromQQ);
            string qq = "", newname = e.Message.Split(' ').Where(x => x.Contains("#")).Last();
            foreach (var cqCode in CqMsg.Parse(e.Message).Contents)
            {
                qq = cqCode.Dictionary["qq"];
                break;
            }
            if (!long.TryParse(qq, out long tag))
            {
                return;
            }
            if (tag == e.FromQQ)
            {
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
            else if (sendMember.PermitType == PermitType.Holder || sendMember.PermitType == PermitType.Manage)
            {
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

        public static void ChangeNewMemberName(string id, CqAddGroupRequestEventArgs e)
        {
            ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
            var player = players.GetPlayer(id);
            var newname = BaseData.Instance.THLevels[player.TownHallLevel] + "本-" + player.Name;
            Common.CqApi.SetGroupMemberNewCard(e.FromGroup, e.FromQQ, newname);
            Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "新人看群文件部落规则，违反任何一条都将会被机票！群昵称已自动改为" + newname);
        }

        public static bool NewMember(string id, CqAddGroupRequestEventArgs e)
        {
            Common.CqApi.AddLoger(LogerLevel.Info_Receive, "部落冲突群管", "接受到新人申请");
            ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
            List<int> levels = new List<int>();
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
                            troopsLV = BaseData.GetTownhallTroopsLV(player.TownHallLevel);
                        }
                        if (troopsLV[troop.Name.Replace(" ", "_")] > troop.Level)
                        {
                            troopFull = false;
                            try
                            {
                                sb.AppendLine(BaseData.Instance.translation[troop.Name.Replace(" ", "_")] + " 还缺" + (troopsLV[troop.Name.Replace(" ", "_")] - troop.Level) + "级");
                                levels.Add(troopsLV[troop.Name.Replace(" ", "_")] - troop.Level);
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
                            troopsLV = BaseData.GetTownhallTroopsLV(player.TownHallLevel);
                        }
                        if (troopsLV[spell.Name.Replace(" ", "_")] > spell.Level)
                        {
                            spellFull = false;
                            try
                            {
                                sb.AppendLine(BaseData.Instance.translation[spell.Name.Replace(" ", "_")] + " 还缺" + (troopsLV[spell.Name.Replace(" ", "_")] - spell.Level) + "级");
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
                if (levels.Count(x => x > 1) > 6)
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
                return true;
            }
        }

        public static void Kick(CqGroupMessageEventArgs e)
        {
            Common.CqApi.AddLoger(LogerLevel.Info_Receive,"部落冲突群管", "接受到踢人指令");
            var sender = Common.CqApi.GetMemberInfo(e.FromGroup, e.FromQQ);
            if(sender.PermitType == PermitType.None)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "已把" + sender.Card + "踢出群聊！他娘的没权限还想踢人？");
                return;
            }
            string qq = "";
            var cqcontent = CqMsg.Parse(e.Message).Contents;
            if (!cqcontent.Any(x => x.Dictionary.ContainsKey("qq")))
            {
                Common.CqApi.AddLoger(LogerLevel.Info_Receive,"部落冲突群管", "没有检测到QQ");
                return;
            }
            foreach (var cqCode in cqcontent)
            {
                qq = cqCode.Dictionary["qq"];
                break;
            }
            Common.CqApi.AddLoger(LogerLevel.Debug, "部落冲突内测", "已检测到QQ号" + qq);
            if (!long.TryParse(qq, out long tag))
            {
                return;
            }
            else
            {
                var member = Common.CqApi.GetMemberInfo(e.FromGroup, tag);
                Common.CqApi.SendGroupMessage(e.FromGroup, "已把" + member.Nick + "|" + member.Card + "踢出群聊！");
                Common.CqApi.SetGroupMemberRemove(e.FromGroup, member.QQId);
            }
        }
    }
}
