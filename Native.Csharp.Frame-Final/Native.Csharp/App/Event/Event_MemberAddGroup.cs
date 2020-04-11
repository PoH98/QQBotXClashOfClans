using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.App.Bot;
using System;
using System.Linq;
using System.IO;

namespace Native.Csharp.App.Event
{
    public class Event_MemberAddGroup : IReceiveAddGroupRequest
    {
        public void ReceiveAddGroupRequest(object sender, CqAddGroupRequestEventArgs e)
        {
            try
            {
                Common.CqApi.AddLoger(LogerLevel.Info, "部落冲突收人", "已收到来自" + e.FromQQ + "的申请, 申请资料为" + e.Message);
                if (e.Message.Contains("#"))
                {
                    var cocid = e.Message.Split('\n').Last().Remove(0,3).Replace(" ", "");
                    if (AdminAPI.NewMember(cocid, e))
                    {
                        Common.CqApi.SetGroupAddRequest(e.ResponseFlag, RequestType.GroupAdd, ResponseType.PASS, "");
                        AdminAPI.ChangeNewMemberName(cocid, e);
                    }
                    else
                    {
                        Common.CqApi.SetGroupAddRequest(e.ResponseFlag, RequestType.GroupAdd, ResponseType.FAIL, "科技不足！");
                    }
                }
                else
                {
                    if (Directory.GetFiles("com.coc.groupadmin\\Blacklist").Any(x => x.EndsWith(e.FromQQ.ToString())))
                    {
                        //在黑名单内，直接拒绝
                        Common.CqApi.SetGroupAddRequest(e.ResponseFlag, RequestType.GroupAdd, ResponseType.FAIL, "已被拉黑！");
                    }
                    Common.CqApi.SetGroupAddRequest(e.ResponseFlag, RequestType.GroupAdd, ResponseType.PASS, "");
                    Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "新人请发玩家标签审核！");
                }
            }
            catch(Exception ex)
            {
                if(ex is NullReferenceException)
                {
                    if (Directory.GetFiles("com.coc.groupadmin\\Blacklist").Any(x => x.EndsWith(e.FromQQ.ToString())))
                    {
                        //在黑名单内，直接拒绝
                        Common.CqApi.SetGroupAddRequest(e.ResponseFlag, RequestType.GroupAdd, ResponseType.FAIL, "已被拉黑！");
                    }
                    Common.CqApi.SetGroupAddRequest(e.ResponseFlag, RequestType.GroupAdd, ResponseType.PASS, "");
                    Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "新人请发玩家标签审核！申请时玩家标签无效！");
                }
                else
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, ex.ToString());
                }
            }
            
        }
    }
}
