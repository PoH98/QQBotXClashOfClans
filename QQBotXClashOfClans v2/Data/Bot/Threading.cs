using CocNET.Interfaces;
using CocNET.Types.Players;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using QQBotXClashOfClans_v2.Data;
using QQBotXClashOfClans_v2.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Unity;

namespace QQBotXClashOfClans_v2
{
    public class Threading
    {
        private Dictionary<long, Thread> War { get; }
        private MiraiHttpSession Session { get; }
        public Threading(MiraiHttpSession session)
        {
            Session = session;
            War = new Dictionary<long, Thread>();
        }
        public void CheckClanWar()
        {
            Logger.Instance.AddLog(LogType.Info, "部落战检测系统启动");
            DateTime lastBackup = DateTime.MinValue;
            do
            {
                try
                {
                    Thread.Sleep(1000);
                    if ((DateTime.Now.Minute == 0 || DateTime.Now.Minute == 30) && DateTime.Now.Second == 0)
                    {
                        ICocCoreClans clan = BaseData.Instance.container.Resolve<ICocCoreClans>();
                        ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
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
                                            Logger.Instance.AddLog(LogType.Info, "正在备份资料...");
                                            //BackUp our game Data
                                            if (!Directory.Exists("com.coc.groupadmin\\BackUp\\" + clanID.KeyName))
                                            {
                                                Directory.CreateDirectory("com.coc.groupadmin\\BackUp\\" + clanID.KeyName);
                                            }
                                            foreach (var file in Directory.GetFiles("com.coc.groupadmin\\" + clanID.KeyName))
                                            {
                                                if(File.Exists(file.Replace("com.coc.groupadmin\\", "com.coc.groupadmin\\BackUp\\")))
                                                {
                                                    File.Delete(file.Replace("com.coc.groupadmin\\", "com.coc.groupadmin\\BackUp\\"));
                                                }
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
                                            if (DateTime.Now.Month == 1 && DateTime.Now.Day == 1 && DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                                            {
                                                //新年快乐
                                                Session.SendGroupMessage(value, new AtAllMessage(), new PlainMessage("我tm祝各位tm的" + DateTime.Now.Year + "新年快乐"));
                                            }
                                            if (!War.ContainsKey(value) || War[value] == null)
                                            {
                                                var clanData = clan.GetCurrentWar(clanID.Value);
                                                if (clanData.State.ToLower() == "preparation")
                                                {
                                                    //New thread
                                                    Thread t = new Thread(() => {
                                                        Session.SendGroupMessage(Convert.ToInt64(clanID.KeyName), new AtAllMessage(), new PlainMessage("部落战已进入准备日！"));
                                                        //Weird error which always more 12 hours
                                                        var wait =  clanData.EndTime.ToLocalTime() - DateTime.Now - new TimeSpan(12, 0, 0);
                                                        Logger.Instance.AddLog(LogType.Debug, "部落战当前准备日，还需要等待" + wait.Days + "天" + wait.Hours + "小时" + wait.Minutes + "分钟" + wait.Seconds + "秒");
                                                        if(wait.TotalSeconds > 0)
                                                        {
                                                            Thread.Sleep(wait);
                                                            Session.SendGroupMessage(Convert.ToInt64(clanID.KeyName), new AtAllMessage(), new PlainMessage("部落战已开始！"));
                                                        }
                                                        return;
                                                    });
                                                    t.IsBackground = true;
                                                    War.Add(value, t);
                                                    t.Start();
                                                }
                                                else if(clanData.State.ToLower() == "inwar")
                                                {
                                                    Thread t = new Thread(() => {
                                                        var wait = clanData.EndTime.ToLocalTime() - DateTime.Now;
                                                        Logger.Instance.AddLog(LogType.Debug, "部落战当前已开始，还需要等待" + wait.Days + "天" + wait.Hours + "小时" + wait.Minutes + "分钟" + wait.Seconds + "秒");
                                                        if(wait.TotalSeconds > 0)
                                                        {
                                                            Thread.Sleep(wait);
                                                            Session.SendGroupMessage(Convert.ToInt64(clanID.KeyName), new AtAllMessage(), new PlainMessage("部落战已结束！"));
                                                        }
                                                        return;
                                                    });
                                                    t.IsBackground = true;
                                                    War.Add(value, t);
                                                    t.Start();
                                                }
                                            }
                                            else if (War[value].ThreadState == ThreadState.Stopped || War[value].ThreadState == ThreadState.Aborted || War[value].ThreadState == ThreadState.Suspended)
                                            {
                                                War[value] = null;
                                                GC.Collect();
                                                var clanData = clan.GetCurrentWar(clanID.Value);
                                                if (clanData.State.ToLower() == "preparation")
                                                {
                                                    //New thread
                                                    Thread t = new Thread(() => {
                                                        Session.SendGroupMessage(Convert.ToInt64(clanID.KeyName), new AtAllMessage(), new PlainMessage("部落战已进入准备日！"));
                                                        //Weird error which always more 12 hours
                                                        var wait = clanData.EndTime.ToLocalTime() - DateTime.Now - new TimeSpan(12, 0, 0);
                                                        Logger.Instance.AddLog(LogType.Debug, "部落战当前准备日，还需要等待" + wait.Days + "天" + wait.Hours + "小时" + wait.Minutes + "分钟" + wait.Seconds + "秒");
                                                        if(wait.TotalSeconds > 0)
                                                        {
                                                            Thread.Sleep(wait);
                                                            Session.SendGroupMessage(Convert.ToInt64(clanID.KeyName), new AtAllMessage(), new PlainMessage("部落战已开始！"));
                                                        }
                                                        return;
                                                    });
                                                    t.IsBackground = true;
                                                    War[value] = t;
                                                    t.Start();
                                                }
                                                else if (clanData.State.ToLower() == "inwar")
                                                {
                                                    Thread t = new Thread(() => {
                                                        var wait = clanData.EndTime.ToLocalTime() - DateTime.Now;
                                                        Logger.Instance.AddLog(LogType.Debug, "部落战当前已开始，还需要等待" + wait.Days + "天" + wait.Hours + "小时" + wait.Minutes + "分钟" + wait.Seconds + "秒");
                                                        if (wait.TotalSeconds > 0)
                                                        {
                                                            Thread.Sleep(wait);
                                                            Session.SendGroupMessage(Convert.ToInt64(clanID.KeyName), new AtAllMessage(), new PlainMessage("部落战已结束！"));
                                                        }
                                                        return;
                                                    });
                                                    t.IsBackground = true;
                                                    War[value] = t;
                                                    t.Start();
                                                }
                                            }
                                            var Members = clan.GetClansMembers(clanID.Value);
                                            var GroupMembers = Session.GetGroupMemberListAsync(value).Result;
                                            foreach(var member in GroupMembers)
                                            {
                                                using var api = new GameAPI(value, member.Id, Session);
                                                foreach (var cd in api.Member.ClanData)
                                                {
                                                    var m = Members.Where(x => x.Tag == cd.ClanID || x.Tag.Replace("0", "O") == cd.ClanID || x.Tag.Replace("1", "I") == cd.ClanID || x.Tag.Replace("0", "O").Replace("1", "I") == cd.ClanID).FirstOrDefault();
                                                    if (m != null)
                                                    {
                                                        cd.InClan = true;
                                                        cd.Name = m.Name;
                                                        cd.LastSeenInClan = DateTime.Now;
                                                    }
                                                    else
                                                    {
                                                        cd.InClan = false;
                                                    }
                                                }
                                                if (api.Member.ClanData.Count == 1 && DateTime.Now.Hour == 0)
                                                {
                                                    Player player = players.GetPlayer(api.Member.ClanData.First().ClanID);
                                                    if (api.Member.Member.Card != BaseData.Instance.THLevels[player.TownHallLevel] + "本-" + player.Name)
                                                    {
                                                        Session.ChangeGroupMemberInfo(api.Member.Member.QQId, value, new GroupMemberCardInfo(BaseData.Instance.THLevels[player.TownHallLevel] + "本-" + player.Name, null));
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Logger.Instance.AddLog(LogType.Error, "群号无法加载，资料出现错误");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Instance.AddLog(LogType.Error, clanID.KeyName + ": " + ex.Message);
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
                    Logger.Instance.AddLog(LogType.Error, "部落战检测出现错误: " + ex.Message);
                }
            }
            while (true);
        }
    }
}
