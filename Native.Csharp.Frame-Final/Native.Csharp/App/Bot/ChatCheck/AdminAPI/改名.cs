using CocNET.Interfaces;
using Native.Csharp.Sdk.Cqp;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Bot
{
    public class 改名:ChatCheckChain
    {
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.StartsWith("/改名"))
            {
                Common.CqApi.AddLoger(LogerLevel.Info_Receive, "部落冲突群管", "接受到改名指令");
                GroupMemberInfo sendMember = Common.CqApi.GetMemberInfo(chat.FromGroup, chat.FromQQ);
                string qq = "", newname;
                foreach (var cqCode in CqMsg.Parse(chat.Message).Contents)
                {
                    qq = cqCode.Dictionary["qq"];
                    break;
                }
                if (!long.TryParse(qq, out long tag))
                {
                    return "我不知道你在艾特个毛线";
                }
                if (chat.Message.Contains(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]))
                {
                    return Common.CqApi.CqCode_At(chat.FromQQ) + "你当我傻？拿部落标签给我查玩家？草你马的";
                }
                if (tag == chat.FromQQ)
                {
                    if (chat.Message.Contains('#'))
                    {
                        newname = chat.Message.Split(' ').Where(x => x.Contains("#")).Last();
                        ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                        var player = players.GetPlayer(newname);
                        if (!string.IsNullOrEmpty(player.Reason))
                        {
                            return "找不到玩家资料或者玩家标签错误: " + player.Reason;
                        }
                        newname = BaseData.Instance.THLevels[player.TownHallLevel] + "本-" + player.Name;
                    }
                    else if (chat.Message.Contains("http"))
                    {
                        newname = chat.Message.Replace("/审核", "").Replace(" ", "");
                        newname = "#" + newname.Remove(0, newname.LastIndexOf("tag=") + 4);
                        if (newname.Contains("&"))
                        {
                            newname = newname.Remove(newname.IndexOf('&'));
                        }
                        ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                        var player = players.GetPlayer(newname);
                        if (!string.IsNullOrEmpty(player.Reason))
                        {
                            return "找不到玩家资料或者玩家标签错误: " + player.Reason;
                        }
                        newname = BaseData.Instance.THLevels[player.TownHallLevel] + "本-" + player.Name;
                    }
                    else
                    {
                        throw new Exception();
                    }
                    Common.CqApi.SetGroupMemberNewCard(chat.FromGroup, tag, newname);
                    return  "搞定！已改称为" + newname;
                }
                else if (sendMember.PermitType == PermitType.Holder || sendMember.PermitType == PermitType.Manage)
                {
                    if (chat.Message.Contains('#'))
                    {
                        newname = chat.Message.Split(' ').Where(x => x.Contains("#")).Last();
                        ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                        var player = players.GetPlayer(newname);
                        if (!string.IsNullOrEmpty(player.Reason))
                        {
                            return "找不到玩家资料或者玩家标签错误: " + player.Reason;
                        }
                        newname = BaseData.Instance.THLevels[player.TownHallLevel] + "本-" + player.Name;
                    }
                    else if (chat.Message.Contains("http"))
                    {
                        newname = chat.Message.Replace("/审核", "").Replace(" ", "");
                        newname = "#" + newname.Remove(0, newname.LastIndexOf("tag=") + 4);
                        if (newname.Contains("&"))
                        {
                            newname = newname.Remove(newname.IndexOf('&'));
                        }
                        ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                        var player = players.GetPlayer(newname);
                        if (!string.IsNullOrEmpty(player.Reason))
                        {
                            return "找不到玩家资料或者玩家标签错误: " + player.Reason;
                        }
                        newname = BaseData.Instance.THLevels[player.TownHallLevel] + "本-" + player.Name;
                    }
                    else
                    {
                        throw new Exception();
                    }
                    Common.CqApi.SetGroupMemberNewCard(chat.FromGroup, tag, newname);
                    return "搞定！已改称为" + newname;
                }
                else
                {
                    return Common.CqApi.CqCode_At(chat.FromQQ) + "你没权限，别把我当脑残！";
                }
            }
            return base.GetReply(chat);
        }
    }
}
