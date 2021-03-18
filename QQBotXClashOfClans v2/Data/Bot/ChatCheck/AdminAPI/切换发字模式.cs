
using Mirai_CSharp.Models;
using QQBotXClashOfClans_v2;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QQBotXClashOfClans_v2.ChatCheck.AdminAPI
{
    public class 切换发字模式 : ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message == "/切换发字模式")
            {
                var sender = chat.Sender;
                if (sender.Permission != GroupPermission.Member)
                {
                    BaseData.Instance.SplitLongText = !BaseData.Instance.SplitLongText;
                    return new IMessageBase[]{new PlainMessage("切换成功！当前设置自动切分长消息是" + (BaseData.Instance.SplitLongText ? "启用" : "关闭"))};
                }
                else
                {
                    return new IMessageBase[]{new PlainMessage("我丢手雷楼老母亲，你没权限设置！")};
                }
            }
            return await base.GetReply(chat);
        }
    }
}
