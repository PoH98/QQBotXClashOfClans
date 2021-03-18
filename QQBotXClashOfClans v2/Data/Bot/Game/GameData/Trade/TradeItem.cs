using Native.Csharp.App.GameData;
using System;

namespace QQBotXClashOfClans_v2.GameData.Trade
{
    [Serializable]
    public class TradeItem
    {
        public string UniqueId { get; set; }

        public GameMember Owner { get; set; }

        public Element Item { get; set; }

        public long Price { get; set; }

        public int Amount { get; set; }
    }
}
