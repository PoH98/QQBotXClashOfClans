using CocNET.Interfaces;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Bot
{
    public class clanapi:ChatCheckChain
    {
        public override IEnumerable<string> GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.ToLower().StartsWith("/部落资料 #"))
            {
                try
                {
                    var cocid = chat.Message.Split(' ').Where(x => x.Contains("#")).Last().Replace(" ", "");
                    ICocCoreClans players = BaseData.Instance.container.Resolve<ICocCoreClans>();
                    var player = players.GetClansMembers(cocid);
                    if (player != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("部落成员数量：" + player.Count);
                        sb.AppendLine("成员列表: ");
                        foreach (var p in player)
                        {
                            sb.AppendLine(p.Name + " : " + p.Tag);
                        }
                        return new string[] { BaseData.TextToImg(sb.ToString()) };
                    }
                    else
                    {
                        return new string[] { "未知的部落冲突ID，无法搜索该部落资料！" };
                    }

                }
                catch (Exception ex)
                {
                    return new string[] { "请确保发送/ClanAPI时是/ClanAPI 玩家标签！错误资料：" + ex.ToString() };
                }
            }
            return base.GetReply(chat);
        }
    }
}
