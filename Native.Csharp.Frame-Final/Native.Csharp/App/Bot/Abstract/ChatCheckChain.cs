using Native.Csharp.App.Bot.Interface;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;

namespace Native.Csharp.App.Bot
{
    public abstract class ChatCheckChain : IChain
    {
        private IChain nextChain;
        public virtual IEnumerable<string> GetReply(CqGroupMessageEventArgs chat)
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
                    return new string[] { "处理指令时发生错误！" + name + ".GetReply()" + "! 错误详情: " + ex.Message };
                }
            }
            else
            {
                return new string[] { };
            }
        }

        public IChain SetNext(IChain chainObject)
        {
            nextChain = chainObject;
            return chainObject;
        }
    }
}
