using CocNET.Interfaces;
using QQBotXClashOfClans_v2.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using System.Threading.Tasks;
using Mirai_CSharp.Models;

namespace QQBotXClashOfClans_v2
{
    public class 清人: ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message.StartsWith("/清人"))
            {
                Logger.Instance.AddLog(LogType.Debug, "接受到检查指令");
                var GroupMember = await chat.Session.GetGroupMemberListAsync(chat.FromGroup);
                var clan = BaseData.Instance.container.Resolve<ICocCoreClans>();
                var result = clan.GetClansMembers(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()].Trim());
                if(result == null)
                {
                    return new IMessageBase[]{new PlainMessage("请检查设置或者绑定部落后才使用此功能！")};
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("不在部落的成员名单: ");
                List<string> noBind = new List<string>();
                foreach (var member in GroupMember)
                {
                    using var api = new GameAPI(chat.FromGroup, member.Id, chat.Session);
                    try
                    {
                        if (api.Member.ClanData.Count < 1)
                        {
                            noBind.Add(api.Member.Member.Card);
                            continue;
                        }
                        if (api.Member.ClanData.Any(x => !x.InClan))
                        {
                            sb.AppendLine(member.Name + ":-");
                            var buffer = api.Member.ClanData;
                            foreach (var data in buffer)
                            {
                                if (!data.InClan)
                                {
                                    if (string.IsNullOrEmpty(data.Name))
                                    {
                                        api.Member.ClanData.Remove(api.Member.ClanData.Where(x => x.ClanID == data.ClanID).First());
                                    }
                                    else
                                    {
                                        try
                                        {
                                            sb.AppendLine(" * " + data.Name + " 已经不在部落" + (DateTime.Now - data.LastSeenInClan.Value).TotalDays.ToString("N2") + "天");
                                        }
                                        catch
                                        {
                                            sb.AppendLine(" * " + data.Name + " 无在部落记录");
                                        }
                                    }


                                }
                            }
                            sb.AppendLine("==============");
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                sb.AppendLine("群里无绑定名单: ");
                foreach(var nb in noBind)
                {
                    sb.AppendLine(nb);
                }
                return new IMessageBase[]{ BaseData.TextToImg(sb.ToString(), chat.Session) };
            }
            return await base.GetReply(chat);
        }
    }
}
