using Native.Csharp.App.Bot.Interface;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Bot
{
    public abstract class ChatCheckChain : IChain
    {
        private IChain nextChain;
        public virtual string GetReply(CqGroupMessageEventArgs chat)
        {
            if(nextChain != null)
            {
                try
                {
                    return nextChain.GetReply(chat);
                }
                catch(Exception ex)
                {
                    var name = nextChain.GetType().Name;
                    return "处理指令时发生错误！" + name + ".GetReply()" + "! 错误详情: " + ex.Message;
                }

            }
            else
            {
                return string.Empty;
            }
        }

        public IChain SetNext(IChain chainObject)
        {
            nextChain = chainObject;
            return chainObject;
        }
    }
}
