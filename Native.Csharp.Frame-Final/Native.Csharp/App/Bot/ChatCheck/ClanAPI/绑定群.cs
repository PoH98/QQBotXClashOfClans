using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System.Collections.Generic;
using System.Linq;

namespace Native.Csharp.App.Bot
{
    public class 绑定群:ChatCheckChain
    {
        public override IEnumerable<string> GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.StartsWith("/绑定群 #"))
            {
                if (Common.CqApi.GetMemberInfo(chat.FromGroup, chat.FromQQ).PermitType != PermitType.None)
                {
                    string clanID = chat.Message.Split(' ').Where(x => x.Contains("#")).Last();
                    BaseData.SetClanID(chat.FromGroup, clanID);
                    return new string[] { Common.CqApi.CqCode_At(chat.FromQQ) + "已绑定" + chat.FromGroup + "为部落ID" + clanID };
                }
                else
                {
                    return new string[] { Common.CqApi.CqCode_At(chat.FromQQ) + "我丢你蕾姆，你没权限用这个功能！" };
                }
            }
            return base.GetReply(chat);
        }
    }
}
