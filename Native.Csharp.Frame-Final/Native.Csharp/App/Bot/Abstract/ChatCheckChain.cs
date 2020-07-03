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
                return nextChain.GetReply(chat);
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
