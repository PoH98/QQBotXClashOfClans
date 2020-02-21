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
            var exit = GameAPI.Instance.members[e.FromGroup].Where(x => x.QQId == e.FromQQ);
            if (exit.Count() > 0)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, exit.ToArray()[0].Card + " 已退群！");
            }
            else
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, e.FromQQ + " 已退群！");
            }
        }
    }
}
