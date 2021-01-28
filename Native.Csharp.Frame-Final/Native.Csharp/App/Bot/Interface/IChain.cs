using Native.Csharp.App.GameData;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System.Collections.Generic;

namespace Native.Csharp.App.Bot.Interface
{
    public interface IChain
    {
        IChain SetNext(IChain chainObject);

        IEnumerable<string> GetReply(CqGroupMessageEventArgs chat);

        void SetMember(GameMember Member);
    }
}
