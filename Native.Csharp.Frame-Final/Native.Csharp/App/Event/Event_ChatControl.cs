using System;
using System.Linq;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using System.Text;
using static Native.Csharp.App.Bot.BaseData;
using Native.Csharp.Sdk.Cqp.Model;
using Native.Csharp.App.Bot;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.App.Bot.Game;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Threading;

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
                        foreach(var message in SplitLongMessage(sb.ToString()))
                        {
                            Thread.Sleep(rnd.Next(500, 3000));
                            Common.CqApi.SendGroupMessage(e.FromGroup, message.Replace("@发送者", Common.CqApi.CqCode_At(e.FromQQ)));
                        }
                            
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        var lines = data[e.Message].Split('\\');
                        foreach (var line in lines)
                        {
                            sb.AppendLine(line);
                        }
                        Random rnd = new Random();
                        foreach (var message in SplitLongMessage(sb.ToString()))
                        {
                            Thread.Sleep(rnd.Next(500, 3000));
                            Common.CqApi.SendGroupMessage(e.FromGroup, message.Replace("@发送者", Common.CqApi.CqCode_At(e.FromQQ)));
                        }
                    }
                }
                else if (e.Message.StartsWith("/"))
                {
                    var result = Instance.chains[0].GetReply(e);
                    if (!string.IsNullOrEmpty(result))
                    {
                        Random rnd = new Random();
                        foreach (var message in SplitLongMessage(result))
                        {
                            Thread.Sleep(rnd.Next(500, 3000));
                            Common.CqApi.SendGroupMessage(e.FromGroup, message);
                        }
                    }
                    else if (!Instance.GameEnabled.Any(x => x == e.FromGroup))
                    {
                        switch (e.Message)
                        {
                            case "/拉霸":
                                new GameAPI(e).JackPot().Dispose();
                                break;
                            case "/寻宝":
                                new GameAPI(e).FindTreasure().Dispose();
                                break;
                            case "/帮助":
                                new GameAPI(e).Help().Dispose();
                                break;
                            case "/工作":
                                new GameAPI(e).MemberWork().Dispose();
                                break;
                            case "/我":
                                new GameAPI(e).MemberCheck().Dispose();
                                break;
                            case "/21点":
                                new GameAPI(e).Member21Point().Dispose();
                                break;
                            case "/排名":
                                new GameAPI(e).GetRank().Dispose();
                                break;
                            default:
                                if (e.Message.StartsWith("/打劫"))
                                {
                                    new GameAPI(e).Robber(e).Dispose();
                                }
                                else if (e.Message.StartsWith("/购买"))
                                {
                                    new GameAPI(e).Shop(e).Dispose();
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

        private string[] SplitLongMessage(string originalMessage)
        {
            var arr = originalMessage.Split('\n');
            int writtenchar = 0;
            List<string> buffer = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach(var line in arr)
            {
                if(line.Length > 0)
                {
                    if (writtenchar > 200)
                    {
                        buffer.Add(sb.ToString());
                        writtenchar = 0;
                        sb.Clear();
                    }
                    writtenchar += line.Length + 1;
                    sb.Append(line + "\n");
                }
            }
            buffer.Add(sb.ToString());
            return buffer.ToArray();
        }
    }
}
