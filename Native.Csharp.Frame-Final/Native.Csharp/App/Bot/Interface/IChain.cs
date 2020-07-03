using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Bot.Interface
{
    public interface IChain
    {
        IChain SetNext(IChain chainObject);

        string GetReply(CqGroupMessageEventArgs chat);
    }
}
