﻿using CocNET.Interfaces;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Bot.ChatCheck.ClanAPI
{
    class 部落成员:ChatCheckChain
    {
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if(chat.Message == "/部落成员")
            {
                if (BaseData.Instance.config["部落冲突"].ContainsKey(chat.FromGroup.ToString()))
                {
                    ICocCoreClans players = BaseData.Instance.container.Resolve<ICocCoreClans>();
                    var player = players.GetClansMembers(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]);
                    if (player != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("部落成员数量：" + player.Count);
                        sb.AppendLine("成员列表: ");
                        foreach (var p in player)
                        {
                            sb.AppendLine(p.Name + " : " + p.Tag);
                        }
                        return "@发送者 您需要的玩家资料在下面：\n@PlayerAPI".Replace("@PlayerAPI", sb.ToString()).Replace("@发送者", Common.CqApi.CqCode_At(chat.FromQQ));
                    }
                    else
                    {
                        return "未知的部落冲突ID，无法搜索该部落资料！";
                    }
                }
                else
                {
                    return "请在config.ini设置好Clan_ID后再继续使用此功能";
                }
            }
            return base.GetReply(chat);
        }
    }
}
