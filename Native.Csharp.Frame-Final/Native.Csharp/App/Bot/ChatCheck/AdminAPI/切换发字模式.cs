using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Enum;

namespace Native.Csharp.App.Bot.ChatCheck.AdminAPI
{
    public class 切换发字模式 : ChatCheckChain
    {
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message == "/切换发字模式")
            {
                var sender = Common.CqApi.GetMemberInfo(chat.FromGroup, chat.FromQQ);
                if (sender.PermitType != PermitType.None)
                {
                    BaseData.Instance.SplitLongText = !BaseData.Instance.SplitLongText;
                    return "切换成功！当前设置自动切分长消息是" + (BaseData.Instance.SplitLongText?"启用":"关闭");
                }
            }
            return base.GetReply(chat);
        }
    }
}
