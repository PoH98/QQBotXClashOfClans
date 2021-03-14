using Native.Csharp.App.Bot.Game;
using System.Linq;
using System.Text;

namespace Native.Csharp.App.GameData
{
    internal class Buff
    {
        GameAPI member;
        public Buff(GameAPI member)
        {
            this.member = member;
        }

        public string AddAttack()
        {
            int el0 = 0, el1 = 0, el2 = 0, el3 = 0, el4 = 0;
            var el0d = member.Member.Inventory.Where(x => x.Element.ID == 0).FirstOrDefault();
            var el1d = member.Member.Inventory.Where(x => x.Element.ID == 1).FirstOrDefault();
            var el2d = member.Member.Inventory.Where(x => x.Element.ID == 2).FirstOrDefault();
            var el3d = member.Member.Inventory.Where(x => x.Element.ID == 3).FirstOrDefault();
            var el4d = member.Member.Inventory.Where(x => x.Element.ID == 4).FirstOrDefault();
            if (el0d != null || el1d != null || el2d != null || el3d != null || el4d != null)
            {
                if (el0d == null)
                {
                    el0d = new InventoryItem() { Element = member.elements.First(x => x.ID == 0), ContainsCount = 0 };
                }
                if (el1d == null)
                {
                    el1d = new InventoryItem() { Element = member.elements.First(x => x.ID == 1), ContainsCount = 0 };
                }
                if (el2d == null)
                {
                    el2d = new InventoryItem() { Element = member.elements.First(x => x.ID == 2), ContainsCount = 0 };
                }
                if (el3d == null)
                {
                    el3d = new InventoryItem() { Element = member.elements.First(x => x.ID == 3), ContainsCount = 0 };
                }
                if (el4d == null)
                {
                    el4d = new InventoryItem() { Element = member.elements.First(x => x.ID == 4), ContainsCount = 0 };
                }
                el0 = el0d.ContainsCount;
                el1 = el1d.ContainsCount;
                el2 = el2d.ContainsCount;
                el3 = el3d.ContainsCount;
                el4 = el4d.ContainsCount;
                if (el0 >= 30)
                {
                    el0d.ContainsCount -= 30;
                    member.Member.BonusDamage += 2;
                    return "强化成功！";
                }
                else if (el1 >= 15)
                {
                    el1d.ContainsCount -= 15;
                    member.Member.BonusDamage += 2;
                    return "强化成功！";
                }
                else if (el2 >= 10)
                {
                    el2d.ContainsCount -= 10;
                    member.Member.BonusDamage += 2;
                    return "强化成功！";
                }
                else if (el3 >= 5)
                {
                    el3d.ContainsCount -= 5;
                    member.Member.BonusDamage += 2;
                    return "强化成功！";
                }
                else if (el4 >= 3)
                {
                    el4d.ContainsCount -= 3;
                    member.Member.BonusDamage += 2;
                    return "强化成功！";
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("强化失败！材料不足！还缺少");
                    if (el0 < 30)
                    {
                        var left = 30 - el0;
                        sb.AppendLine(el0d.Element.Name + ":" + left);
                    }
                    if (el1 < 15)
                    {
                        var left = 15 - el1;
                        sb.AppendLine(el1d.Element.Name + ":" + left);
                    }
                    if (el2 < 10)
                    {
                        var left = 10 - el2;
                        sb.AppendLine(el2d.Element.Name + ":" + left);
                    }
                    if (el3 < 5)
                    {
                        var left = 5 - el3;
                        sb.AppendLine(el3d.Element.Name + ":" + left);
                    }
                    if (el4 < 3)
                    {
                        var left = 3 - el4;
                        sb.Append(el4d.Element.Name + ":" + left);
                    }
                    return sb.ToString();
                }
            }
            else
            {
                if (el0d == null)
                {
                    el0d = new InventoryItem() { Element = member.elements.First(x => x.ID == 0), ContainsCount = 0 };
                }
                if (el1d == null)
                {
                    el1d = new InventoryItem() { Element = member.elements.First(x => x.ID == 1), ContainsCount = 0 };
                }
                if (el2d == null)
                {
                    el2d = new InventoryItem() { Element = member.elements.First(x => x.ID == 2), ContainsCount = 0 };
                }
                if (el3d == null)
                {
                    el3d = new InventoryItem() { Element = member.elements.First(x => x.ID == 3), ContainsCount = 0 };
                }
                if (el4d == null)
                {
                    el4d = new InventoryItem() { Element = member.elements.First(x => x.ID == 4), ContainsCount = 0 };
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("强化失败！材料不足！还缺少");
                if (el0 < 30)
                {
                    var left = 30 - el0;
                    sb.AppendLine(el0d.Element.Name + ":" + left);
                }
                if (el1 < 15)
                {
                    var left = 15 - el1;
                    sb.AppendLine(el1d.Element.Name + ":" + left);
                }
                if (el2 < 10)
                {
                    var left = 10 - el2;
                    sb.AppendLine(el2d.Element.Name + ":" + left);
                }
                if (el3 < 5)
                {
                    var left = 5 - el3;
                    sb.AppendLine(el3d.Element.Name + ":" + left);
                }
                if (el4 < 3)
                {
                    var left = 3 - el4;
                    sb.Append(el4d.Element.Name + ":" + left);
                }
                return sb.ToString();
            }
        }

        public string AddHP()
        {
            int el0 = 0, el1 = 0, el2 = 0, el3 = 0, el4 = 0;
            var el0d = member.Member.Inventory.Where(x => x.Element.ID == 5).FirstOrDefault();
            var el1d = member.Member.Inventory.Where(x => x.Element.ID == 6).FirstOrDefault();
            var el2d = member.Member.Inventory.Where(x => x.Element.ID == 7).FirstOrDefault();
            var el3d = member.Member.Inventory.Where(x => x.Element.ID == 8).FirstOrDefault();
            var el4d = member.Member.Inventory.Where(x => x.Element.ID == 9).FirstOrDefault();
            if (el0d != null || el1d != null || el2d != null || el3d != null || el4d != null)
            {
                if (el0d == null)
                {
                    el0d = new InventoryItem() { Element = member.elements.First(x => x.ID == 5), ContainsCount = 0 };
                }
                if (el1d == null)
                {
                    el1d = new InventoryItem() { Element = member.elements.First(x => x.ID == 6), ContainsCount = 0 };
                }
                if (el2d == null)
                {
                    el2d = new InventoryItem() { Element = member.elements.First(x => x.ID == 7), ContainsCount = 0 };
                }
                if (el3d == null)
                {
                    el3d = new InventoryItem() { Element = member.elements.First(x => x.ID == 8), ContainsCount = 0 };
                }
                if (el4d == null)
                {
                    el4d = new InventoryItem() { Element = member.elements.First(x => x.ID == 9), ContainsCount = 0 };
                }
                el0 = el0d.ContainsCount;
                el1 = el1d.ContainsCount;
                el2 = el2d.ContainsCount;
                el3 = el3d.ContainsCount;
                el4 = el4d.ContainsCount;
                if (el0 >= 30)
                {
                    el0d.ContainsCount -= 30;
                    member.Member.BonusHP += 10;
                    return "强化成功！";
                }
                else if (el1 >= 15)
                {
                    el1d.ContainsCount -= 15;
                    member.Member.BonusHP += 10;
                    return "强化成功！";
                }
                else if (el2 >= 10)
                {
                    el2d.ContainsCount -= 10;
                    member.Member.BonusHP += 10;
                    return "强化成功！";
                }
                else if (el3 >= 5)
                {
                    el3d.ContainsCount -= 5;
                    member.Member.BonusHP += 10;
                    return "强化成功！";
                }
                else if (el4 >= 3)
                {
                    el4d.ContainsCount -= 3;
                    member.Member.BonusHP += 10;
                    return "强化成功！";
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("强化失败！材料不足！还缺少");
                    if (el0 < 30)
                    {
                        var left = 30 - el0;
                        sb.AppendLine(el0d.Element.Name + ":" + left);
                    }
                    if (el1 < 15)
                    {
                        var left = 15 - el1;
                        sb.AppendLine(el1d.Element.Name + ":" + left);
                    }
                    if (el2 < 10)
                    {
                        var left = 10 - el2;
                        sb.AppendLine(el2d.Element.Name + ":" + left);
                    }
                    if (el3 < 5)
                    {
                        var left = 5 - el3;
                        sb.AppendLine(el3d.Element.Name + ":" + left);
                    }
                    if (el4 < 3)
                    {
                        var left = 3 - el4;
                        sb.Append(el4d.Element.Name + ":" + left);
                    }
                    return sb.ToString();
                }
            }
            else
            {
                if (el0d == null)
                {
                    el0d = new InventoryItem() { Element = member.elements.First(x => x.ID == 5), ContainsCount = 0 };
                }
                if (el1d == null)
                {
                    el1d = new InventoryItem() { Element = member.elements.First(x => x.ID == 6), ContainsCount = 0 };
                }
                if (el2d == null)
                {
                    el2d = new InventoryItem() { Element = member.elements.First(x => x.ID == 7), ContainsCount = 0 };
                }
                if (el3d == null)
                {
                    el3d = new InventoryItem() { Element = member.elements.First(x => x.ID == 8), ContainsCount = 0 };
                }
                if (el4d == null)
                {
                    el4d = new InventoryItem() { Element = member.elements.First(x => x.ID == 9), ContainsCount = 0 };
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("强化失败！材料不足！还缺少");
                if (el0 < 30)
                {
                    var left = 30 - el0;
                    sb.AppendLine(el0d.Element.Name + ":" + left);
                }
                if (el1 < 15)
                {
                    var left = 15 - el1;
                    sb.AppendLine(el1d.Element.Name + ":" + left);
                }
                if (el2 < 10)
                {
                    var left = 10 - el2;
                    sb.AppendLine(el2d.Element.Name + ":" + left);
                }
                if (el3 < 5)
                {
                    var left = 5 - el3;
                    sb.AppendLine(el3d.Element.Name + ":" + left);
                }
                if (el4 < 3)
                {
                    var left = 3 - el4;
                    sb.Append(el4d.Element.Name + ":" + left);
                }
                return sb.ToString();
            }
        }
    }
}
