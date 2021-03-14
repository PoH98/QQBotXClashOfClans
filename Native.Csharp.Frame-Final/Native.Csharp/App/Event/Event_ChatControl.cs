using System;
using System.Linq;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using System.Text;
using static Native.Csharp.App.Bot.BaseData;
using Native.Csharp.App.Bot;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.App.Bot.Game;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace Native.Csharp.App.Event
{
    public class GroupMessage : IReceiveGroupMessage
    {
        public void ReceiveGroupMessage(object sender, CqGroupMessageEventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
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
                            Thread.Sleep(rnd.Next(1000, 4000));
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
                            Thread.Sleep(rnd.Next(1000, 4000));
                            Common.CqApi.SendGroupMessage(e.FromGroup, message.Replace("@发送者", Common.CqApi.CqCode_At(e.FromQQ)));
                        }
                    }
                }
                else if (e.Message.StartsWith("/"))
                {
                    GameAPI Member = new GameAPI(e);
                    Instance.chains[0].SetMember(Member.Member);
                    var result = Instance.chains[0].GetReply(e);
                    if (result.Count() > 0)
                    {
                        Random rnd = new Random();
                        foreach (var r in result)
                        {
                            if (r.Contains("[bmp:"))
                            {
                                Regex regex = new Regex(@"\[bmp:(\S*)\]");
                                var match = regex.Match(r);
                                var fileName = match.Groups[1].Value;
                                var tempr = r.Replace(match.Groups[0].Value, "");
                                Common.CqApi.SendGroupMessage(e.FromGroup, tempr + Common.CqApi.CqCode_Image(fileName));
                            }
                            else
                            {
                                Common.CqApi.SendGroupMessage(e.FromGroup, r);
                            }
                            Thread.Sleep(rnd.Next(1000, 3000));
                        }

                    }
                    else if (!Instance.GameEnabled.Any(x => x == e.FromGroup))
                    {
                        switch (e.Message)
                        {
                            case "/拉霸":
                                Member.JackPot();
                                break;
                            case "/寻宝":
                                Member.FindTreasure();
                                break;
                            case "/帮助":
                                Member.Help();
                                break;
                            case "/工作":
                                Member.MemberWork();
                                break;
                            case "/我":
                                Member.MemberCheck();
                                break;
                            case "/21点":
                                Member.Member21Point();
                                break;
                            case "/排名":
                                Member.GetRank();
                                break;
                            case "/合成A":
                            case "/合成a":
                                Member.Buff("A");
                                break;
                            case "/合成H":
                            case "/合成h":
                                Member.Buff("H");
                                break;
                            case "/拍卖场":
                                Member.ListTrade();
                                break;
                            default:
                                if (e.Message.StartsWith("/打劫"))
                                {
                                    Member.Robber(e);
                                }
                                else if (e.Message.StartsWith("/购买"))
                                {
                                    Member.Shop(e);
                                }
                                else if (e.Message.StartsWith("/技能"))
                                {
                                    Member.SkillShop(e);
                                }
                                else if (e.Message.StartsWith("/拍卖场购买"))
                                {
                                    Member.PurchaseTradeItem(e.Message);
                                }
                                else if (e.Message.StartsWith("/拍卖场出售"))
                                {
                                    Member.SellTradeItem(e.Message);
                                }
                                break;
                        }
                    }
                    Member.Dispose();
                }
                sw.Stop();
                Common.CqApi.AddLoger(LogerLevel.Debug, "部落冲突群管", "指令处理完毕！已使用" + sw.ElapsedMilliseconds + "毫秒");
                if (Directory.Exists("Buffer"))
                {
                    foreach (var file in Directory.GetFiles("Buffer"))
                    {
                        FileInfo info = new FileInfo(file);
                        if((DateTime.Now - info.CreationTime).TotalDays > 1)
                        {
                            info.Delete();
                        }
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
            if (!Instance.SplitLongText)
            {
                return new string[]{ originalMessage };
            }
            var arr = originalMessage.Split('\n');
            int writtenchar = 0;
            List<string> buffer = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach(var line in arr)
            {
                if(line.Length > 0)
                {
                    if (writtenchar > 50)
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
