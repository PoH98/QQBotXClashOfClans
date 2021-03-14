using Native.Csharp.App.Bot;
using Native.Csharp.App.Bot.Game;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using System;
using System.IO;
using System.Linq;

namespace Native.Csharp.App.Event
{
    class Event_MemberLeave : IReceiveGroupMemberLeave
    {
        public void ReceiveGroupMemberLeave(object sender, CqGroupMemberDecreaseEventArgs e)
        {
            try
            {
                GameAPI api = new GameAPI(e.FromGroup, e.BeingOperateQQ);
                Common.CqApi.SendGroupMessage(e.FromGroup, api.Member.Member.Nick + " 已退群！");
                api.Dispose();
                if (File.Exists("com.coc.groupadmin\\" + e.FromGroup + "\\" + e.BeingOperateQQ + ".bin"))
                {
                    File.Delete("com.coc.groupadmin\\" + e.FromGroup + "\\" + e.BeingOperateQQ + ".bin");
                }
            }
            catch(Exception ex)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "退群处理错误", ex.ToString());
            }
        }
    }
}
