using CocNET.Interfaces;
using Native.Csharp.App.Bot.Game;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;

namespace Native.Csharp.App.Bot
{
    public class 清人: ChatCheckChain
    {
        public override IEnumerable<string> GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.StartsWith("/清人"))
            {
                Common.CqApi.AddLoger(LogerLevel.Info_Receive, "部落冲突群管", "接受到检查指令");
                var GroupMember = Common.CqApi.GetMemberList(chat.FromGroup);
                var clan = BaseData.Instance.container.Resolve<ICocCoreClans>();
                var result = clan.GetClansMembers(BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()].Trim());
                if(result == null)
                {
                    return new string[] { "请检查设置或者绑定部落后才使用此功能！" };
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("不在部落的成员名单: ");
                foreach (var member in GroupMember)
                {
                    using (var api = new GameAPI(chat.FromGroup, member.QQId))
                    {
                        try
                        {
                            if (api.Member.ClanData.Count < 1)
                            {
                                continue;
                            }
                            if (api.Member.ClanData.Any(x => !x.InClan))
                            {
                                sb.AppendLine(member.Card + ":-");
                                foreach (var data in api.Member.ClanData)
                                {
                                    if (!data.InClan)
                                    {
                                        try
                                        {
                                            sb.AppendLine(" * " + data.Name + " 已经不在部落" + (DateTime.Now - data.LastSeenInClan.Value).TotalDays + "天");
                                        }
                                        catch
                                        {
                                            sb.AppendLine(" * " + data.Name + " 无在部落记录");
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
                }
                /*sb.AppendLine("不在群的成员名单: ");
                foreach (var id in result)
                {
                    bool Found = false;
                    foreach(var mem in GroupMember)
                    {
                        try
                        {
                            using (var m = new GameAPI(chat.FromQQ, mem.QQId))
                            {
                                Found = m.Member.ClanData.Any(y => y.ClanID.Trim() == id.Tag.Trim());
                                if (Found)
                                {
                                    break;
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                    if (!Found)
                    {
                        sb.AppendLine(id.ClanRank + ":" + id.Name);
                    }
                }*/
                return new string[] { BaseData.TextToImg(sb.ToString()) };
            }
            return base.GetReply(chat);
        }
    }
}
