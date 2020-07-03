﻿using System;
using System.Linq;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using System.Text;
using static Native.Csharp.App.Bot.BaseData;
using Native.Csharp.Sdk.Cqp.Model;
using Native.Csharp.App.Bot;
using Native.Csharp.Sdk.Cqp.Enum;

namespace Native.Csharp.App.Event
{
    public class GroupMessage : IReceiveGroupMessage
    {
        public void ReceiveGroupMessage(object sender, CqGroupMessageEventArgs e)
        {
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
                        Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString().Replace("@发送者", Common.CqApi.CqCode_At(e.FromQQ)));
                    }
                }
                else if (e.Message.StartsWith("/"))
                {
                    var result = Instance.chains[0].GetReply(e);
                    if (!string.IsNullOrEmpty(result))
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, result);
                        return;
                    }
                    else if (Instance.GameEnabled)
                    {
                        switch (e.Message)
                        {
                            case "/拉霸":
                                GameAPI.JackPot(e);
                                break;
                            case "/寻宝":
                                GameAPI.FindTreasure(e);
                                break;
                            case "/帮助":
                                GameAPI.Help(e);
                                break;
                            case "/工作":
                                GameAPI.MemberWork(e);
                                break;
                            case "/我":
                                GameAPI.MemberCheck(e);
                                break;
                            case "/21点":
                                GameAPI.Member21Point(e);
                                break;
                            case "/排名":
                                GameAPI.GetRank(e);
                                break;
                            case "/打Boss":
                            case "/打boss":
                                BossFight.Fight(e);
                                break;

                            default:
                                if (e.Message.StartsWith("/打劫"))
                                {
                                    GameAPI.Robber(e);
                                }
                                else if (e.Message.StartsWith("/购买"))
                                {
                                    GameAPI.Shop(e);
                                }
                                break;
                        }
                    }
                }
                GroupMemberInfo me = Common.CqApi.GetMemberInfo(e.FromGroup, Common.CqApi.GetLoginQQ());
                if (me.PermitType == PermitType.Holder || me.PermitType == PermitType.Manage)
                {
                    try
                    {
                        GroupMemberInfo sendMember = Common.CqApi.GetMemberInfo(e.FromGroup, e.FromQQ);
                        Common.CqApi.AddLoger(LogerLevel.Debug, "发消息成员资料", "已加群" + (DateTime.Now - sendMember.JoiningTime).Days + "天");
                        if (sendMember.PermitType == PermitType.None && (DateTime.Now - sendMember.JoiningTime).Days < 15)
                        {
                            foreach (var keyvalue in valuePairs(configType.禁止词句))
                            {
                                if (e.Message.Contains(keyvalue.Key))
                                {
                                    Common.CqApi.RepealMessage(e.Id);
                                    Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + " 你敢触发禁止词？你大爷的，草，跟我互喷啊！");
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
            catch (Exception ex)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "出现错误，请稍后再试！错误详情：" + ex.ToString());
            }
        }
    }
}
