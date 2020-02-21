using System;
using System.Linq;
using CocNET.Interfaces;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using System.Text;
using static Native.Csharp.App.Bot.BaseData;
using Native.Csharp.Sdk.Cqp.Model;
using Native.Csharp.App.Bot;
using CocNET.Types.Clans.LeagueWar;
using System.Net;
using Native.Csharp.Sdk.Cqp.Enum;

namespace Native.Csharp.App.Event
{
    public class GroupMessage : IReceiveGroupMessage
    {
        public void ReceiveGroupMessage(object sender, CqGroupMessageEventArgs e)
        {
            ShareEvent(e, e.FromGroup);
            try
            {
                var data = valuePairs(configType.自动回复);
                if (data.Keys.Contains(e.Message))
                {
                    if (data[e.Message].Contains('|'))
                    {
                        var messages = data[e.Message].Split('|');
                        Random rnd = new Random();
                        StringBuilder sb = new StringBuilder();
                        var lines = messages[rnd.Next(0, messages.Length)].Split('\\');
                        foreach (var line in lines)
                        {
                            sb.AppendLine(line);
                        }
                        Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString().Replace("@发送者", Common.CqApi.CqCode_At(e.FromQQ)));
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        var lines = data[e.Message].Split('\\');
                        foreach (var line in lines)
                        {
                            sb.AppendLine(line);
                        }
                        Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString().Replace("@发送者",  Common.CqApi.CqCode_At(e.FromQQ)));
                    }
                }
                else
                {
                    if (e.Message.StartsWith("/PlayerAPI") && e.Message.Contains("#"))
                    {
                        PlayerAPI.GetPlayer(e.Message.Split(' ')[1].Replace(" ", ""), e);
                    }
                    else if (e.Message.StartsWith("/ClanAPI") && e.Message.Contains("#"))
                    {
                        ClanAPI.GetClan(e.Message.Split(' ')[1].Replace(" ", ""), e);
                    }
                    else if (e.Message == "/部落战")
                    {
                        if (Instance.config["部落冲突"].ContainsKey(e.FromGroup.ToString()))
                        {
                            ClanAPI.GetWar(e);
                        }
                        else
                        {
                            Common.CqApi.SendGroupMessage(e.FromGroup, "请在config.ini设置好Clan_ID后再继续使用此功能");
                        }
                    }
                    else if (e.Message == "/部落成员")
                    {
                        if (Instance.config["部落冲突"].ContainsKey(e.FromGroup.ToString()))
                        {
                            ClanAPI.GetMember(e);
                        }
                        else
                        {
                            Common.CqApi.SendGroupMessage(e.FromGroup, "请在config.ini设置好Clan_ID后再继续使用此功能");
                        }
                    }
                    else if (e.Message == "/部落战剩余进攻")
                    {
                        if (Instance.config["部落冲突"].ContainsKey(e.FromGroup.ToString()))
                        {
                            ClanAPI.GetWarLeft(e);
                        }
                        else
                        {
                            Common.CqApi.SendGroupMessage(e.FromGroup, "请在config.ini设置好Clan_ID后再继续使用此功能");
                        }
                    }
                    else if (e.Message == "/清人")
                    {
                        if (Instance.config["部落冲突"].ContainsKey(e.FromGroup.ToString()))
                        {
                            AdminAPI.CheckMember(e);
                        }
                        else
                        {
                            Common.CqApi.SendGroupMessage(e.FromGroup, "请在config.ini设置好Clan_ID后再继续使用此功能");
                        }
                    }
                    else if (e.Message.StartsWith("/改名 "))
                    {
                        try
                        {
                            AdminAPI.ChangeName(e);
                        }
                        catch
                        {
                            Common.CqApi.SendGroupMessage(e.FromGroup, "确保/改名格式为 /改名 @成员 新昵称 或者 /改名 @成员 #部落冲突玩家标签");
                        }
                    }
                    else if (e.Message.StartsWith("/审核 #"))
                    {
                        PlayerAPI.CheckMember(e);
                    }
                    else if (e.Message.StartsWith("/联赛"))
                    {
                        ClanAPI.GetLeagueWar(e);
                    }
                    else if (e.Message == "/拉霸")
                    {

                    }
                    else if (e.Message.StartsWith("/下载 "))
                    {
                        string version = e.Message.Remove(0, 4);
                        WebClient wc = new WebClient();
                        var html = wc.DownloadString("http://leiren520.com/download/index.html");
                        var cocdiv = html.Substring(html.IndexOf("COC下载"));
                        cocdiv = cocdiv.Substring(0, cocdiv.IndexOf("多开器下载"));
                        bool found = false;
                        StringBuilder sb = new StringBuilder();
                        foreach (LinkItem i in LinkFinder.Find(cocdiv))
                        {
                            sb.AppendLine(i.Text);
                            if (i.Text.Contains(version) || version.Contains(i.Text))
                            {
                                found = true;
                                Common.CqApi.SendGroupMessage(e.FromGroup,  Common.CqApi.CqCode_At(e.FromQQ) + "你要的部落冲突下载链接：" + i.Href);
                                break;
                            }
                        }
                        if (!found)
                        {
                            Common.CqApi.SendGroupMessage(e.FromGroup,  Common.CqApi.CqCode_At(e.FromQQ) + "哈？你确定你要的是部落冲突？我这里只有:\n" + sb.ToString());
                        }
                    }
                    else if (e.Message.StartsWith("/踢"))
                    {
                        AdminAPI.Kick(e);
                    }
                    else if (e.Message.StartsWith("/绑定群 #"))
                    {
                        if(Common.CqApi.GetMemberInfo(e.FromGroup, e.FromQQ).PermitType != PermitType.None)
                        {
                            string clanID = e.Message.Split(' ').Where(x => x.Contains("#")).Last();
                            SetClanID(e.FromGroup, clanID);
                            Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "已绑定" + e.FromGroup + "为部落ID" + clanID);
                        }
                        else
                        {
                            Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "我丢你蕾姆，你没权限用这个功能！");
                        }
                    }
                    else if (e.Message == "/工作")
                    {
                        GameAPI.MemberWork(e);
                    }
                    else if(e.Message == "/我")
                    {
                        GameAPI.MemberCheck(e);
                    }
                    else if(e.Message == "/21点")
                    {
                        GameAPI.Member21Point(e);
                    }
                    else if(e.Message == "/排名")
                    {
                        GameAPI.GetRank(e);
                    }
                    else if (e.Message.StartsWith("/打劫"))
                    {
                        GameAPI.Robber(e);
                    }
                    else if (e.Message.StartsWith("/购买"))
                    {
                        GameAPI.Shop(e);
                    }
                    else
                    {
                        GroupMemberInfo me = Common.CqApi.GetMemberInfo(e.FromGroup, Common.CqApi.GetLoginQQ());
                        if (me.PermitType == PermitType.Holder || me.PermitType == PermitType.Manage)
                        {
                            try
                            {
                                GroupMemberInfo sendMember = Common.CqApi.GetMemberInfo(e.FromGroup, e.FromQQ);
                                if (sendMember.PermitType != PermitType.Holder && sendMember.PermitType != PermitType.Manage && (sendMember.JoiningTime - DateTime.Now).Days < 15)
                                {
                                    foreach (var keyvalue in valuePairs(configType.禁止词句))
                                    {
                                        if (e.Message.Contains(keyvalue.Key))
                                        {
                                            Common.CqApi.RepealMessage(e.Id);
                                            Common.CqApi.SendGroupMessage(e.FromGroup,  Common.CqApi.CqCode_At(e.FromQQ) + " 你敢触发禁止词？你大爷的，草，跟我互喷啊！");
                                            if (int.TryParse(keyvalue.Value, out int ticks))
                                            {
                                                if (ticks > 0)
                                                {
                                                    Common.CqApi.SetGroupBanSpeak(e.FromGroup, e.FromQQ, new TimeSpan(0, 0, ticks));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "出现错误，请稍后再试！错误详情：" + ex.ToString());
            }
        }
    }
}
