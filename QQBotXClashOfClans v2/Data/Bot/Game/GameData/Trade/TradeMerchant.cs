using QQBotXClashOfClans_v2.Game;
using Native.Csharp.App.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Mirai_CSharp;

namespace QQBotXClashOfClans_v2.GameData.Trade
{
    internal class TradeMerchant:IDisposable
    {
        private readonly List<TradeItem> tradeItems = new List<TradeItem>();
        public void Dispose()
        {
            SaveData();
        }

        private void SaveData()
        {
            var writer = new XmlSerializer(typeof(List<TradeItem>));
            //Clear the contents before rewrite the data back
            File.Delete("com.coc.groupadmin\\trade.xml");
            using var wfile = new FileStream("com.coc.groupadmin\\trade.xml", FileMode.CreateNew);
            writer.Serialize(wfile, tradeItems);
        }

        ~TradeMerchant()
        {
            SaveData();
        }

        internal TradeMerchant()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<TradeItem>));
            if (!File.Exists("com.coc.groupadmin\\trade.xml"))
            {
                using StreamWriter stream = new StreamWriter("com.coc.groupadmin\\trade.xml");
                serializer.Serialize(stream, tradeItems);
            }
            using (FileStream stream = new FileStream("com.coc.groupadmin\\trade.xml", FileMode.Open))
            {
                tradeItems = (List<TradeItem>)serializer.Deserialize(stream);
            }
        }

        internal string ShowList()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("发送指令\"/拍卖场购买 购买物品的ID\"进行购买\n你也可以发送指令\"/拍卖场出售 材料名字 价格(纯数字) 数量(纯数字)\"进行上架想贩卖的物品");
            sb.AppendLine("|ID                |商品           |价格         |数量    |出售者                                  |");
            foreach(var item in tradeItems)
            {
                sb.AppendLine("|" + item.UniqueId + "|" + item.Item.Name.PadRight(10) + "|" + item.Price.ToString("N0").PadRight(10) + "|" + item.Amount.ToString().PadRight(8) + "|" + item.Owner.Member.Card.PadRight(30) + "|");
            }
            return sb.ToString();
        }

        internal string AddItem(Element element, long price, int amount, GameAPI owner)
        {
            if (amount < 0 || price < 0)
            {
                return "你卖个寂寞？";
            }
            if (!owner.Member.Inventory.Any(x => x.Element.Name == element.Name && x.ContainsCount >= amount))
            {
                return "你没有足够数量的商品贩卖！";
            }
            if (price > 99999)
            {
                return "商场拒收如此贵重的物品！请把价格调低！";
            }
            owner.Member.Inventory.First(x => x.Element.Name == element.Name).ContainsCount -= amount;
            var newitem = new TradeItem
            {
                UniqueId = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "").Substring(0, 8).ToUpper(),
                Item = element,
                Owner = owner.Member,
                Price = price,
                Amount = amount
            };
            tradeItems.Add(newitem);
            return "商品已经上架！";
        }

        internal string PurchaseItem(GameAPI buyer, string guid, MiraiHttpSession Session)
        {
            var item = tradeItems.FirstOrDefault(x => x.UniqueId == guid);
            if(item == null)
            {
                return "找不到该商品！";
            }
            if(buyer.Member.Member.QQId == item.Owner.Member.QQId)
            {
                if (!buyer.Member.Inventory.Any(x => x.Element.Name == item.Item.Name))
                {
                    buyer.Member.Inventory.Add(new InventoryItem() { ContainsCount = item.Amount, Element = item.Item });
                }
                else
                {
                    buyer.Member.Inventory.First(x => x.Element.Name == item.Item.Name).ContainsCount += item.Amount;
                }
                tradeItems.Remove(item);
            }
            else
            {
                if (buyer.Member.Cash < item.Price)
                {
                    return "你没钱买个屁！";
                }
                buyer.Member.Cash -= item.Price;
                if (!buyer.Member.Inventory.Any(x => x.Element.Name == item.Item.Name))
                {
                    buyer.Member.Inventory.Add(new InventoryItem() { ContainsCount = item.Amount, Element = item.Item });
                }
                else
                {
                    buyer.Member.Inventory.First(x => x.Element.Name == item.Item.Name).ContainsCount += item.Amount;
                }
                using (GameAPI owner = new GameAPI(item.Owner.Member.GroupId, item.Owner.Member.QQId, Session))
                {
                    owner.Member.Cash += item.Price;
                }
                tradeItems.Remove(item);
            }

            return "购买成功！";
        }
    }
}
