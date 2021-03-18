using CocNET.Interfaces;
using Mirai_CSharp.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace QQBotXClashOfClans_v2
{
    public class 部落战剩余进攻:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if(chat.Message == "/部落战剩余进攻")
            {
                ICocCoreClans clan = BaseData.Instance.container.Resolve<ICocCoreClans>();
                var clanData = clan.GetCurrentWar(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]);
                if (!string.IsNullOrEmpty(clanData.Reason))
                {
                    return new IMessageBase[]{new PlainMessage("无法获取部落资料！" + clanData.Reason)};
                }
                else
                {
                    if (clanData.Reason == "inMaintenance")
                    {
                        return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage(" 当前服务器在维护！")};
                    }
                    else if (clanData.State == "inWar")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("\n你要的部落战资料：\n");
                        foreach (var Member in clanData.Clan.Members)
                        {
                            if (Member.Attacks == null)
                            {
                                sb.Append(Member.Name + " " + Member.Tag + "\n");
                            }
                        }
                        sb.Append("战斗日结束时间：" + clanData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                        return new IMessageBase[]{ new AtMessage(chat.FromQQ), BaseData.TextToImg(sb.ToString(),chat.Session) };
                    }
                    else
                    {
                        return new IMessageBase[]{ new AtMessage(chat.FromQQ),  new PlainMessage(" 当前部落不在战斗日！(未开战或准备日)")};
                    }
                }
            }
            return await base.GetReply(chat);
        }
    }
}
