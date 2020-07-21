using CocNET.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

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
                        GameAPI.SaveData();
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
                                            GameAPI.GetGroupMembers(value);
                                            var clanData = clan.GetCurrentWar(clanID.Value);
                                            var members = clan.GetClansMembers(clanID.Value);
                                            if (string.IsNullOrEmpty(clanData.Message))
                                            {
                                                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "部落战检测", clanData.State);
                                                if (BaseData.Instance.LastClanWarStatus != clanData.State)
                                                {
                                                    string status = "";
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
                                                    if (!string.IsNullOrEmpty(status))
                                                    {
                                                        Common.CqApi.SendGroupMessage(Convert.ToInt64(clanID.KeyName), Common.CqApi.CqCode_At(-1) + "部落战" + status);
                                                    }
                                                    BaseData.Instance.LastClanWarStatus = clanData.State;
                                                }
                                            }
                                            else
                                            {
                                                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "部落战检测", clanData.Message);
                                            }
                                            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "部落成员检测", "正在读取部落当前成员名单");
                                            var groupnamelist = new 清人().getGroupMemberNameList(value);
                                            foreach (var mem in groupnamelist)
                                            {
                                                if (members.Any(x => x.Name.Contains(mem)))
                                                {
                                                    var _member = GameAPI.Instance.gameMembers[value].Where(x => x.Member.Card.Contains(mem));
                                                    if (_member != null && _member.Count() > 0)
                                                    {
                                                        _member.First().LastSeenInClan = DateTime.Now;
                                                    }
                                                }
                                            }
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
    }
}
