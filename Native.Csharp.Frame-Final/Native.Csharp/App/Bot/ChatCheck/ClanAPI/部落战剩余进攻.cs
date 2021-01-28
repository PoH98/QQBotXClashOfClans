using CocNET.Interfaces;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace Native.Csharp.App.Bot
{
    public class 部落战剩余进攻:ChatCheckChain
    {
        public override IEnumerable<string> GetReply(CqGroupMessageEventArgs chat)
        {
            if(chat.Message == "/部落战剩余进攻")
            {
                ICocCoreClans clan = BaseData.Instance.container.Resolve<ICocCoreClans>();
                var clanData = clan.GetCurrentWar(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]);
                if (!string.IsNullOrEmpty(clanData.Reason))
                {
                    return new string[] { "无法获取部落资料！" + clanData.Reason };
                }
                else
                {
                    if (clanData.Reason == "inMaintenance")
                    {
                        return new string[] { Common.CqApi.CqCode_At(chat.FromQQ) + " 当前服务器在维护！" };
                    }
                    else if (clanData.State == "inWar")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(Common.CqApi.CqCode_At(chat.FromQQ) + "\n你要的部落战资料：\n");
                        foreach (var Member in clanData.Clan.Members)
                        {
                            if (Member.Attacks == null)
                            {
                                sb.Append(Member.Name + " " + Member.Tag + "\n");
                            }
                        }
                        sb.Append("战斗日结束时间：" + clanData.EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
                        return new string[] { BaseData.TextToImg(sb.ToString()) };
                    }
                    else
                    {
                        return new string[] { Common.CqApi.CqCode_At(chat.FromQQ) + " 当前部落不在战斗日！(未开战或准备日)" };
                    }
                }
            }
            return base.GetReply(chat);
        }
    }
}
