using Native.Csharp.App.Bot;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using System.Linq;

namespace Native.Csharp.App.Event
{
    class Event_MemberLeave : IReceiveGroupMemberLeave
    {
        public void ReceiveGroupMemberLeave(object sender, CqGroupMemberDecreaseEventArgs e)
        {
            try
            {
                var exit = GameAPI.Instance.gameMembers[e.FromGroup].Where(x => x.Member.QQId == e.BeingOperateQQ);
                if (exit.Count() > 0)
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, exit.ToArray()[0].Member.Card + " 已退群！");
                }
                else
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, e.BeingOperateQQ + " 已退群！");
                }
                var member = GameAPI.Instance.gameMembers[e.FromGroup].Where(x => x.Member.QQId == e.BeingOperateQQ).FirstOrDefault();
                if(member != null)
                    GameAPI.Instance.gameMembers[e.FromGroup].Remove(member);
            }
            catch
            {

            }
        }
    }
}
