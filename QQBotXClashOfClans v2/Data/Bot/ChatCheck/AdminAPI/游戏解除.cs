
using Mirai_CSharp.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QQBotXClashOfClans_v2.ChatCheck.AdminAPI
{
    public class 游戏解除:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if(chat.Message == "/游戏解除")
            {
                var sender = chat.Sender;
                if (sender.Permission == GroupPermission.Administrator || sender.Permission == GroupPermission.Owner)
                {
                    if (!BaseData.Instance.GameEnabled.Any(x => x == chat.FromGroup))
                    {
                        File.AppendAllText("XGame.txt", chat.FromGroup + "\n");
                        BaseData.Instance.GameEnabled.Add(chat.FromGroup);
                    }
                    return new IMessageBase[]{new PlainMessage("群游戏已取消")};
                }
                else
                {
                    return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage("艹你麻痹，没权限解除你妹！")};
                }
            }
            else if(chat.Message == "/游戏启动")
            {
                var sender = chat.Sender;
                if (sender.Permission == GroupPermission.Administrator || sender.Permission == GroupPermission.Owner)
                {
                    if (BaseData.Instance.GameEnabled.Any(x => x == chat.FromGroup))
                    {
                        File.WriteAllText("XGame.txt", File.ReadAllText("XGame.txt").Replace(chat.FromGroup.ToString(), ""));
                        BaseData.Instance.GameEnabled.Remove(chat.FromGroup);
                    }
                    return new IMessageBase[]{new PlainMessage("群游戏已启动")};
                }
                else
                {
                    return new IMessageBase[] { new AtMessage(chat.FromQQ), new PlainMessage("艹你麻痹，没权限解除你妹！") };
                }
            }
            return await base.GetReply(chat);
        }
    }
}
