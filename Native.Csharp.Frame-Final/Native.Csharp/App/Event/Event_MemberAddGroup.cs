using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.App.Bot;

namespace Native.Csharp.App.Event
{
    public class Event_MemberAddGroup : IReceiveAddGroupRequest
    {
        public void ReceiveAddGroupRequest(object sender, CqAddGroupRequestEventArgs e)
        {
            Common.CqApi.AddLoger(LogerLevel.Info, "部落冲突收人", "已收到来自" + e.FromQQ + "的申请");
            if (e.Message.Contains("#"))
            {
                var cocid = e.Message.Replace(" ", "");
                if (AdminAPI.NewMember(cocid, e))
                {
                    Common.CqApi.SetGroupAddRequest(e.ResponseFlag, RequestType.GroupAdd , ResponseType.PASS, "");
                }
                else
                {
                    Common.CqApi.SetGroupAddRequest(e.ResponseFlag, RequestType.GroupAdd, ResponseType.FAIL, "");
                }
            }
            else
            {
                Common.CqApi.SetGroupAddRequest(e.ResponseFlag, RequestType.GroupAdd, ResponseType.PASS, "");
                Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "新人请发玩家标签审核！");
            }
        }
    }
}
