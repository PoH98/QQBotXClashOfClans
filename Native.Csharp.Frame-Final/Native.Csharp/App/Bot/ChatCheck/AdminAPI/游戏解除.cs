using Native.Csharp.Sdk.Cqp.EventArgs;
using System.IO;
using System.Linq;

namespace Native.Csharp.App.Bot.ChatCheck.AdminAPI
{
    class 游戏解除:ChatCheckChain
    {
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if(chat.Message == "/游戏解除")
            {
                var sender = Common.CqApi.GetMemberInfo(chat.FromGroup, chat.FromQQ);
                if (sender.PermitType == Sdk.Cqp.Enum.PermitType.Holder || sender.PermitType == Sdk.Cqp.Enum.PermitType.Manage)
                {
                    if (!BaseData.Instance.GameEnabled.Any(x => x == chat.FromGroup))
                    {
                        File.AppendAllText("XGame.txt", chat.FromGroup + "\n");
                        BaseData.Instance.GameEnabled.Add(chat.FromGroup);
                    }
                    return "群游戏已取消";
                }
                else
                {
                    return Common.CqApi.CqCode_At(chat.FromQQ) + "艹你麻痹，没权限解除你妹！";
                }
            }
            else if(chat.Message == "/游戏启动")
            {
                var sender = Common.CqApi.GetMemberInfo(chat.FromGroup, chat.FromQQ);
                if (sender.PermitType == Sdk.Cqp.Enum.PermitType.Holder || sender.PermitType == Sdk.Cqp.Enum.PermitType.Manage)
                {
                    if (BaseData.Instance.GameEnabled.Any(x => x == chat.FromGroup))
                    {
                        File.WriteAllText("XGame.txt", File.ReadAllText("XGame.txt").Replace(chat.FromGroup.ToString(), ""));
                        BaseData.Instance.GameEnabled.Remove(chat.FromGroup);
                    }
                    return "群游戏已启动";
                }
                else
                {
                    return Common.CqApi.CqCode_At(chat.FromQQ) + "艹你麻痹，没权限启动你妹！";
                }
            }
            return base.GetReply(chat);
        }
    }
}
