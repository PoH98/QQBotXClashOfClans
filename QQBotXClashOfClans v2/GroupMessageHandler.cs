using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;
using QQBotXClashOfClans_v2.Game;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static QQBotXClashOfClans_v2.BaseData;
using Console = Colorful.Console;

namespace QQBotXClashOfClans_v2
{
    public class GroupMessageHandler : IGroupMessage
    {
        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e)
        {
            if(e.Chain[1] is PlainMessage)
            {
                var plain = e.Chain[1] as PlainMessage;
                lock (Instance.LogLocker)
                {
                    Console.Write("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "] ", Color.Yellow);
                    Console.Write("[MESSAGE] ", Color.ForestGreen);
                    Console.Write("[" + e.Sender.Group.Name + "]: ", Color.Orange);
                    Console.Write("[" + e.Sender.Name + "]: ", Color.Cyan);
                    Console.WriteLine(plain.Message.Replace("\n", "\\n"));
                }
                var data = valuePairs(configType.自动回复);
                if (data.Keys.Contains(plain.Message))
                {

                    if (data[plain.Message].Contains('|'))
                    {
                        var messages = data[plain.Message].Split('|');
                        Random rnd = new Random();
                        StringBuilder sb = new StringBuilder();
                        var lines = messages[rnd.Next(0, messages.Length)].Split('\\');
                        foreach (var line in lines)
                        {
                            sb.AppendLine(line);
                        }
                        foreach (var message in SplitLongMessage(sb.ToString()))
                        {
                            if (message.Contains("@发送者"))
                            {
                                List<IMessageBase> mb = new List<IMessageBase>();
                                var splits = message.Split("@发送者");
                                foreach(var split in splits)
                                {
                                    if (string.IsNullOrWhiteSpace(split) || string.IsNullOrEmpty(split))
                                    {
                                        continue;
                                    }
                                    mb.Add(new AtMessage(e.Sender.Id));
                                    mb.Add(new PlainMessage(split));
                                }
                                await session.SendGroupMessageAsync(e.Sender.Group.Id, mb.ToArray());
                            }
                            else
                            {
                                await session.SendGroupMessageAsync(e.Sender.Group.Id, new IMessageBase[] { new PlainMessage(message) });
                            }
                            Thread.Sleep(rnd.Next(1000, 4000));
                        }
                    }
                    else
                    {

                        StringBuilder sb = new StringBuilder();
                        var lines = data[plain.Message].Split('\\');
                        foreach (var line in lines)
                        {
                            sb.AppendLine(line);
                        }
                        Random rnd = new Random();
                        foreach (var message in SplitLongMessage(sb.ToString()))
                        {
                            if (message.Contains("@发送者"))
                            {
                                List<IMessageBase> mb = new List<IMessageBase>();
                                var splits = message.Split("@发送者");
                                foreach (var split in splits)
                                {
                                    if (string.IsNullOrWhiteSpace(split) || string.IsNullOrEmpty(split))
                                    {
                                        continue;
                                    }
                                    mb.Add(new AtMessage(e.Sender.Id));
                                    mb.Add(new PlainMessage(split));
                                }
                                await session.SendGroupMessageAsync(e.Sender.Group.Id, mb.ToArray());
                            }
                            else
                            {
                                await session.SendGroupMessageAsync(e.Sender.Group.Id, new IMessageBase[] { new PlainMessage(message) });
                            }
                            Thread.Sleep(rnd.Next(1000, 4000));
                        }
                    }
                }
                else if (plain.Message.StartsWith("/"))
                {
                    using GameAPI Member = new GameAPI(new ChainEventArgs() { Message = plain.Message, MessageChain = e.Chain, Sender = e.Sender, Session = session });
                    Instance.chains[0].SetMember(Member.Member);
                    var result = await Instance.chains[0].GetReply(new ChainEventArgs() { Message = plain.Message, MessageChain = e.Chain, Sender = e.Sender, Session = session });
                    if (result.Count() > 0)
                    {
                        await session.SendGroupMessageAsync(e.Sender.Group.Id, result.ToArray());
                    }
                    else if (!Instance.GameEnabled.Any(x => x == e.Sender.Group.Id))
                    {
                        switch (plain.Message)
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
                                if (plain.Message.StartsWith("/打劫"))
                                {
                                    Member.Robber(new ChainEventArgs() { Message = plain.Message, MessageChain = e.Chain, Sender = e.Sender, Session = session });
                                }
                                else if (plain.Message.StartsWith("/购买"))
                                {
                                    Member.Shop(new ChainEventArgs() { Message = plain.Message, MessageChain = e.Chain, Sender = e.Sender, Session = session });
                                }
                                else if (plain.Message.StartsWith("/技能"))
                                {
                                    Member.SkillShop(new ChainEventArgs() { Message = plain.Message, MessageChain = e.Chain, Sender = e.Sender, Session = session });
                                }
                                else if (plain.Message.StartsWith("/拍卖场购买"))
                                {
                                    Member.PurchaseTradeItem(plain.Message);
                                }
                                else if (plain.Message.StartsWith("/拍卖场出售"))
                                {
                                    Member.SellTradeItem(plain.Message);
                                }
                                break;
                        }
                    }
                }
                return true;
            }
            else
            {
                lock (Instance.LogLocker)
                {
                    Console.Write("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "] ", Color.Yellow);
                    Console.Write("[MESSAGE] ", Color.ForestGreen);
                    Console.Write("[" + e.Sender.Group.Name + "]: ", Color.FloralWhite);
                    Console.Write("[" + e.Sender.Name + "]: ", Color.Cyan);
                    for (int x = 1; x < e.Chain.Length; x++)
                    {
                        Console.Write(e.Chain[x].ToString().Replace("\n", "\\n"));
                    }
                    Console.WriteLine();
                }

            }
            return false;
        }
        private string[] SplitLongMessage(string originalMessage)
        {
            if (!Instance.SplitLongText)
            {
                return new string[] { originalMessage };
            }
            var arr = originalMessage.Split('\n');
            int writtenchar = 0;
            List<string> buffer = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (var line in arr)
            {
                if (line.Length > 0)
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

    public class GroupRequestHandler : IGroupApply
    {
        public GroupRequestHandler()
        {
        }

        public async Task<bool> GroupApply(MiraiHttpSession session, IGroupApplyEventArgs e)
        {
            try
            {
                Logger.Instance.AddLog(LogType.Info, "已收到来自" + e.FromQQ + "的申请, 申请资料为" + e.Message);
                if (e.Message.Contains("#"))
                {
                    var cocid = e.Message.Split('\n').Last().Remove(0, 3).Replace(" ", "");
                    if (await AdminAPI.NewMember(cocid,  new Data.ApplyEventArgs { EventArgs = e, Session = session }))
                    {
                        await session.HandleGroupApplyAsync(e, GroupApplyActions.Allow);
                        await AdminAPI.ChangeNewMemberName(cocid, new Data.ApplyEventArgs() { EventArgs = e, Session = session });
                        return true;
                    }
                    else
                    {
                        await session.HandleGroupApplyAsync(e, GroupApplyActions.Deny, "科技不足");
                        return true;
                    }
                }
                else
                {
                    if (Directory.GetFiles("com.coc.groupadmin\\Blacklist").Any(x => x.EndsWith(e.FromQQ.ToString())))
                    {
                        //在黑名单内，直接拒绝
                        await session.HandleGroupApplyAsync(e, GroupApplyActions.Deny, "已被拉黑");
                        return true;
                    }
                    await session.HandleGroupApplyAsync(e, GroupApplyActions.Allow);
                    await session.SendGroupMessageAsync(e.FromGroup,new AtMessage(e.FromQQ), new PlainMessage("新人请发玩家标签审核！"));
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    if (Directory.GetFiles("com.coc.groupadmin\\Blacklist").Any(x => x.EndsWith(e.FromQQ.ToString())))
                    {
                        //在黑名单内，直接拒绝
                        await session.HandleGroupApplyAsync(e, GroupApplyActions.Deny, "已被拉黑");
                        return true;
                    }
                    await session.HandleGroupApplyAsync(e, GroupApplyActions.Allow);
                    await session.SendGroupMessageAsync(e.FromGroup, new AtMessage(e.FromQQ), new PlainMessage("新人请发玩家标签审核！审核时玩家标签无效！"));
                    return true;
                }
                else
                {
                    await session.SendGroupMessageAsync(e.FromGroup, new AtMessage(e.FromQQ), new PlainMessage(ex.Message));
                    return true;
                }
            }
        }
    }

    public class GroupExitHandler : IGroupMemberPositiveLeave
    {
        public async Task<bool> GroupMemberPositiveLeave(MiraiHttpSession session, IGroupMemberPositiveLeaveEventArgs e)
        {
            Console.Write("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "] ", Color.Yellow);
            Console.Write("[LEAVE]    ", Color.Firebrick);
            Console.Write("[" + e.Member.Group.Name + "]: ", Color.Orange);
            Console.WriteLine(e.Member.Name + "已退群");
            if (File.Exists("com.coc.groupadmin\\" + e.Member.Group.Id + "\\" + e.Member.Id + ".bin"))
            {
                File.Delete("com.coc.groupadmin\\" + e.Member.Group.Id + "\\" + e.Member.Id + ".bin");
            }
            await session.SendGroupMessageAsync(e.Member.Group.Id, new PlainMessage(e.Member.Name + "已退群！"));
            return true;
        }
    }
}
