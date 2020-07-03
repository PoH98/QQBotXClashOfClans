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
                string qq = "", newname = chat.Message.Split(' ').Where(x => x.Contains("#")).Last();
                foreach (var cqCode in CqMsg.Parse(chat.Message).Contents)
                {
                    qq = cqCode.Dictionary["qq"];
                    break;
                }
                if (!long.TryParse(qq, out long tag))
                {
                    return string.Empty;
                }
                if (tag == chat.FromQQ)
                {
                    if (newname.Contains('#'))
                    {
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
                    if (newname.Contains('#'))
                    {
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
