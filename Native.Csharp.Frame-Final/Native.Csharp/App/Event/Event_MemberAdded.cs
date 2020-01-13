using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;

namespace Native.Csharp.App.Event
{
    public class Event_MemberAdded : IReceiveGroupMemberPass
    {
        public void ReceiveGroupMemberPass(object sender, CqGroupMemberIncreaseEventArgs e)
        {
            
        }
    }
}
