using Mirai_CSharp.Models;
using Native.Csharp.App.GameData;
using QQBotXClashOfClans_v2;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QQBotXClashOfClans_v2.Interface
{
    public interface IChain
    {
        IChain SetNext(IChain chainObject);

        Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat);

        void SetMember(GameMember Member);
    }
}
