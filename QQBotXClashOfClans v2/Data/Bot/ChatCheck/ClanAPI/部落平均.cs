using CocNET.Interfaces;
using Mirai_CSharp.Models;
using System;
using System.Collections.Generic;
using Unity;
using System.Threading.Tasks;
using System.Text;

namespace QQBotXClashOfClans_v2.Data.Bot.ChatCheck.ClanAPI
{
    public class 部落平均 : ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if(chat.Message == "/部落平均")
            {
                ICocCoreClans clan = BaseData.Instance.container.Resolve<ICocCoreClans>();
                var clanData = clan.GetClans(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()]);
                if (!string.IsNullOrEmpty(clanData.Reason))
                {
                    return new IMessageBase[] { new PlainMessage("无法获取部落资料！" + clanData.Reason) };
                }
                StringBuilder sb = new StringBuilder();
                long donated = 0, donationsReceived = 0;
                foreach(var member in clanData.MemberList)
                {
                    donated += member.Donations;
                    donationsReceived += member.DonationsReceived;
                }
                sb.AppendLine("部落平均胜率: " + ((double)(((double)clanData.WarWins / ((double)clanData.WarWins + (double)clanData.WarLosses + (double)clanData.WarTies)) * 100)).ToString("0.00") + "%");
                sb.AppendLine("部落平均捐兵: " + donated / clanData.MemberList.Count);
                sb.Append("部落平均收兵: " + donationsReceived / clanData.MemberList.Count);
                return new IMessageBase[] { new PlainMessage(sb.ToString()) };
            }
            return await base.GetReply(chat);
        }
    }
}
