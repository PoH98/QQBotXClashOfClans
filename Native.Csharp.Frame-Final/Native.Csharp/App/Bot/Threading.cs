using CocNET.Interfaces;
using CocNET.Types.Other;
using Native.Csharp.App.Bot.Game;
using Native.Csharp.App.GameData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

namespace Native.Csharp.App.Bot
{
    public class Threading
    {
        public static void CheckClanWar()
        {
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "部落冲突检测", "部落战检测系统启动");
            DateTime lastBackup = DateTime.MinValue;
            do
            {
                try
                {
                    Thread.Sleep(1000);
                    if ((DateTime.Now.Minute == 0 || DateTime.Now.Minute == 30) && DateTime.Now.Second == 0)
                    {
                        ICocCoreClans clan = BaseData.Instance.container.Resolve<ICocCoreClans>();
                        try
                        {
                            foreach (var clanID in BaseData.Instance.config["部落冲突"])
                            {
                                if (clanID.KeyName.All(char.IsDigit))
                                {
                                    if (lastBackup.Date != DateTime.Now.Date)
                                    {
                                        try
                                        {
                                            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "部落游戏", "正在备份资料...");
                                            //BackUp our game Data
                                            if (!Directory.Exists("com.coc.groupadmin\\BackUp\\" + clanID.KeyName))
                                            {
                                                Directory.CreateDirectory("com.coc.groupadmin\\BackUp\\" + clanID.KeyName);
                                            }
                                            foreach (var file in Directory.GetFiles("com.coc.groupadmin\\" + clanID.KeyName))
                                            {
                                                File.Copy(file, file.Replace("com.coc.groupadmin\\", "com.coc.groupadmin\\BackUp\\"));
                                            }
                                            lastBackup = DateTime.Now;
                                        }
                                        catch
                                        {

                                        }
                                    }
                                    try
                                    {
                                        if(long.TryParse(clanID.KeyName, out long value))
                                        {
                                            var clanData = clan.GetCurrentWar(clanID.Value);
                                            var members = clan.GetClansMembers(clanID.Value);
                                            if (string.IsNullOrEmpty(clanData.Message))
                                            {
                                                string status = "";
                                                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "部落战检测", clanData.State);
                                                if (!BaseData.Instance.LastClanWarStatus.ContainsKey(value))
                                                {
                                                    BaseData.Instance.LastClanWarStatus.Add(value, string.Empty);
                                                }
                                                if (BaseData.Instance.LastClanWarStatus[value] != clanData.State)
                                                {
                                                    switch (clanData.State)
                                                    {
                                                        case "inWar":
                                                            status = "已开始！";
                                                            break;
                                                        case "warEnded":
                                                            status = "已结束！";
                                                            break;
                                                        case "preparation":
                                                            status = "已进入准备日！";
                                                            break;
                                                    }
                                                    if (!string.IsNullOrEmpty(status) && !string.IsNullOrEmpty(BaseData.Instance.LastClanWarStatus[value]))
                                                    {
                                                        Common.CqApi.SendGroupMessage(Convert.ToInt64(clanID.KeyName), Common.CqApi.CqCode_At(-1) + "部落战" + status);
                                                    }
                                                    BaseData.Instance.LastClanWarStatus[value] = clanData.State;
                                                }
                                            }
                                            else
                                            {
                                                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "部落战检测", clanData.Message);
                                            }
                                            UpdateMemberInClanStatus(value, members);
                                        }
                                        else
                                        {
                                            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "部落成员检测", "群号无法加载，资料出现错误");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "部落战检测", clanID.KeyName + ": " + ex.ToString());
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                catch(Exception ex)
                {
                    Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "部落冲突检测", "部落战检测出现错误: " + ex.ToString());
                }
            }

            while (Common.IsRunning) ;
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "部落冲突检测", "部落战检测系统停止");
        }

        internal static void UpdateMemberInClanStatus(long groupID, List<Member> members)
        {
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "部落成员检测", "正在读取部落当前成员名单");
            var groupnamelist = new 清人().getGroupMemberNameList(groupID);
            List<GameAPI> gameMembers = new List<GameAPI>();
            XmlSerializer reader = new XmlSerializer(typeof(GameMember));
            var files = Directory.GetFiles("com.coc.groupadmin\\" + groupID);
            foreach (var file in files)
            {
                var playerID = long.Parse(file.Remove(0, file.LastIndexOf('\\') + 1).Replace(".bin", ""));
                gameMembers.Add(new GameAPI(groupID, playerID));
            }
            foreach (var mem in groupnamelist)
            {
                if (members.Any(x => x.Name.Contains(mem)))
                {
                    var _member = gameMembers.Where(x => x.member.Member.Card.Contains(mem));
                    if (_member != null && _member.Count() > 0)
                    {
                        _member.First().member.LastSeenInClan = DateTime.Now;
                        _member.First().Dispose();
                    }
                }
            }
            gameMembers.Clear();
        }
    }
}
