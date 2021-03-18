using Mirai_CSharp.Models;
using QQBotXClashOfClans_v2.Interface;
using Native.Csharp.App.GameData;
using QQBotXClashOfClans_v2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QQBotXClashOfClans_v2
{
    public abstract class ChatCheckChain : IChain
    {
        private IChain nextChain;
        internal GameMember Member { get; private set; }
        public virtual async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if(nextChain != null)
            {
                try
                {
                    return await nextChain.GetReply(chat);
                }
                catch(Exception ex)
                {
                    var name = nextChain.GetType().Name;
                    return new[] { new PlainMessage("处理指令时发生错误！" + name + ".GetReply()" + "! 错误详情: " + ex.Message) };
                }
            }
            else
            {
                return new IMessageBase[] { };
            }
        }

        public IChain SetNext(IChain chainObject)
        {
            nextChain = chainObject;
            return chainObject;
        }

        public virtual void SetMember(GameMember Member)
        {
            if(nextChain != null)
            {
                try
                {
                    nextChain.SetMember(Member);
                }
                catch (Exception ex)
                {
                    var name = nextChain.GetType().Name;
                    throw new Exception("处理指令时发生错误！" + name + ".SetMember()" + "! 错误详情: " + ex.Message);
                }
            }
            this.Member = Member;
        }
    }
}
