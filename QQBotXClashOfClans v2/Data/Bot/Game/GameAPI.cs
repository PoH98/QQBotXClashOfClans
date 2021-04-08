using QQBotXClashOfClans_v2.GameData;
using Native.Csharp.App.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Mirai_CSharp.Models;
using Mirai_CSharp;
using QQBotXClashOfClans_v2.Data;

namespace QQBotXClashOfClans_v2.Game
{
    internal class GameAPI :IDisposable
    {
        internal GameMember Member { get; private set; }
        private readonly string groupPath;
        private readonly string MemberFile;
        private int Round = 0;
        internal List<Element> elements = new List<Element>();
        private MiraiHttpSession Session { get; }
        public void Dispose()
        {
            SaveData();
        }

        internal GameAPI(ChainEventArgs e)
        {
            Session = e.Session;
            groupPath = "com.coc.groupadmin\\" + e.FromGroup;
            MemberFile = "com.coc.groupadmin\\" + e.FromGroup + "\\" + e.FromQQ + ".bin";
            if (!Directory.Exists(groupPath))
            {
                Directory.CreateDirectory(groupPath);
            }
            if (!File.Exists(MemberFile))
            {
                var Member = new GroupMemberInfo() { GroupId = e.FromGroup, QQId = e.FromQQ, Card = e.Sender.Name };
                GameMember gameMember;
                gameMember = new GameMember { Member = Member, Cash = 100, Exp = 100, Work = Work.给野蛮人刷背, Checked = DateTime.MinValue, Combo = 0, CurrentHP = 100, weapon = new None(), Robbed = DateTime.MinValue, PlayTime = DateTime.MinValue };
                var writer = new XmlSerializer(typeof(GameMember));
                using StreamWriter stream = new StreamWriter(MemberFile);
                writer.Serialize(stream, gameMember);
            }
            try
            {
                XmlSerializer reader = new XmlSerializer(typeof(GameMember));
                using (FileStream stream = new FileStream(MemberFile, FileMode.Open))
                {
                    this.Member = (GameMember)reader.Deserialize(stream);
                    this.Member.Member = new GroupMemberInfo() { GroupId = e.FromGroup, QQId = e.FromQQ, Card = e.Sender.Name };
                }
                foreach (Type type in Assembly.GetAssembly(typeof(Element)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Element))))
                {
                    elements.Add((Element)Activator.CreateInstance(type));
                }
            }
            catch(Exception ex)
            {
                e.Session.SendGroupMessage(e.FromGroup, new PlainMessage(e.FromQQ + "资料损毁，错误资料:" + ex.Message));
            }
        }


        internal GameAPI(long group, long qqID, MiraiHttpSession Session)
        {
            this.Session = Session;
            groupPath = "com.coc.groupadmin\\" + group;
            MemberFile = "com.coc.groupadmin\\" + group + "\\" + qqID + ".bin";
            if (!Directory.Exists(groupPath))
            {
                Directory.CreateDirectory(groupPath);
            }
            if (!File.Exists(MemberFile))
            {
                var Member = new GroupMemberInfo() { GroupId = group, QQId = qqID, Card = Session.GetGroupMemberInfoAsync(qqID, group).Result.Name };
                GameMember gameMember;
                gameMember = new GameMember { Member = Member, Cash = 100, Exp = 100, Work = Work.给野蛮人刷背, Checked = DateTime.MinValue, Combo = 0, CurrentHP = 100, weapon = new None(), Robbed = DateTime.MinValue, PlayTime = DateTime.MinValue };
                var writer = new XmlSerializer(typeof(GameMember));
                using StreamWriter stream = new StreamWriter(MemberFile);
                writer.Serialize(stream, gameMember);
            }
            else
            {
                //Weird error
                if (File.ReadAllText(MemberFile).Contains("</GameMember>d>"))
                {
                    File.WriteAllText(MemberFile, File.ReadAllText(MemberFile).Replace("</GameMember>d>", ""));
                }
            }
            try
            {
                XmlSerializer reader = new XmlSerializer(typeof(GameMember));
                using (FileStream stream = new FileStream(MemberFile, FileMode.Open))
                {
                    this.Member = (GameMember)reader.Deserialize(stream);
                }
                foreach (Type type in Assembly.GetAssembly(typeof(Element)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Element))))
                {
                    elements.Add((Element)Activator.CreateInstance(type));
                }
            }
            catch (Exception ex)
            {
                Session.SendGroupMessage(group, new PlainMessage(qqID + "资料损毁，错误资料:" + ex.Message));
            }
        }

        private void SaveData()
        {
            if (Member == null)
            {
                Logger.Instance.AddLog(LogType.Debug, "未知错误，Member为Null!");
                return;
            }
            elements.Clear();
            var writer = new XmlSerializer(typeof(GameMember));
            //Clear the contents before rewrite the data back
            File.Delete(MemberFile);
            using var wfile = new FileStream(MemberFile, FileMode.CreateNew);
            writer.Serialize(wfile, Member);
            Logger.Instance.AddLog(LogType.Debug, "数据储存完毕！");
        }

        internal GameAPI Member21Point()
        {
            if (Member == null)
            {
                return this;
            }
            if (Member.PlayTime == null)
            {
                Member.PlayTime = DateTime.MinValue;
            }
            var nextPlay = Member.PlayTime.AddMinutes(15);
            if (DateTime.Now < nextPlay)
            {
                var wait = nextPlay - DateTime.Now;
                Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("由于你过于积极的想要赌博，被保安抓了出去，请在" + wait.Minutes + "分钟" + wait.Seconds + "秒后尝试！"));
                return this;
            }
            if (Member.Cash >= 50)
            {
                Member.PlayTime = DateTime.Now;
                Member.Cash -= 50;
                Random rnd = new Random();
                int player = 0, me = 0;
                do
                {
                    player += rnd.Next(1, 10);
                }
                while (player < 16);
                do
                {
                    me += rnd.Next(1, 10);
                }
                while (me < 16 || me < player);
                if (player > 21)
                {
                    Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("已炸，当前点数为" + player));
                }
                else if (player == 21)
                {
                    Member.Cash += 150;
                    Member.Exp += 150;
                    Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("拿到了21点！可喜可贺！"));
                    if (Member.Exp > SharedData.Instance.需要经验值[Member.Work] &&  Member.Work != Enum.GetValues(typeof(Work)).Cast<Work>().Last())
                    {
                        Member.Work = Member.Work.Next();
                        Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("已升级啦！接下来的工作为" + Member.Work.ToString() + ", 工资为" + SharedData.Instance.工资[Member.Work] + "金币！"));
                    }
                }
                else
                {
                    string result = "当前点数为" + player + "，庄家点数为" + me + "!";
                    if (me < player || me > 21)
                    {
                        Member.Cash += 100;
                        Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage(result + "恭喜玩家获胜！可喜可贺！"));
                    }
                    else if (me == player)
                    {
                        Member.Cash += 50;
                        Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage(result + "打成平手！"));
                    }
                    else
                    {
                        Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage(result + "很遗憾, 玩家败北！"));
                    }
                }
            }
            else
            {
                Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("没钱还想赌博？去去去！"));
            }
            return this;
        }

        internal GameAPI FindTreasure()
        {
            if (Member == null)
            {
                return this;
            }
            if (Member.TreasurePlayTime == null)
            {
                Member.TreasurePlayTime = DateTime.MinValue;
            }
            var nextPlay = Member.TreasurePlayTime.AddMinutes(20);
            if (DateTime.Now < nextPlay)
            {
                var wait = nextPlay - DateTime.Now;
                Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("你才刚回到家又想出门，结果不小心被被窝缠住无法动弹。请在" + wait.Minutes + "分钟" + wait.Seconds + "秒后尝试！"));
                return this;
            }
            Member.TreasurePlayTime = DateTime.Now;
            Random rnd = new Random();
            var result = rnd.Next(0, (SharedData.Instance.TreasureFindingSuccess.Length + SharedData.Instance.TreasureFindingFailed.Length) - 1);
            if (result >= SharedData.Instance.TreasureFindingSuccess.Length)
            {
                result -= SharedData.Instance.TreasureFindingSuccess.Length;
                Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage(SharedData.Instance.TreasureFindingFailed[result]));
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                var coin = rnd.Next(10, 50);
                sb.Append(SharedData.Instance.TreasureFindingSuccess[result].Replace("%G%", coin.ToString()));
                var item = Convert.ToDecimal(rnd.NextDouble());
                var atde = rnd.Next(0,10);
                Element get = null;
                if(atde > 5)
                {
                    //Attack
                    get = elements.Where(x => x.ElementType == ElementType.Attack).Where(x => item <= x.DropRate).OrderBy(x => x.DropRate).FirstOrDefault();
                }
                else
                {
                    get = elements.Where(x => x.ElementType == ElementType.Defence).Where(x => item <= x.DropRate).OrderBy(x => x.DropRate).FirstOrDefault();
                }
                if (get != null)
                {
                    if(Member.Inventory == null)
                    {
                        Member.Inventory = new List<InventoryItem>();
                    }
                    var meminv = Member.Inventory.FirstOrDefault(x => x.Element.ID == get.ID);
                    if (meminv == null)
                    {
                        Member.Inventory.Add(new InventoryItem() { ContainsCount = 1, Element = get });
                    }
                    else
                    {
                        meminv.ContainsCount++;
                    }
                    sb.Append("\n并且意外的捡到了一个" + get.Name + "!");
                }
                Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage(sb.ToString()));
                Member.Cash += coin;
            }
            return this;
        }

        internal GameAPI MemberWork()
        {
            if (Member == null)
            {
                return this;
            }
            Logger.Instance.AddLog(LogType.Debug, Member.Checked.Date + " " + DateTime.Now.Date);
            if (Member.Checked.Date != DateTime.Now.Date)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("已" + Member.Work.ToString() + ", 获取了" + SharedData.Instance.工资[Member.Work] + "金币！");
                if (Member.Checked.AddDays(1).Date == DateTime.Now.Date)
                {
                    Member.Combo++;
                    if (Member.Combo > 0)
                    {
                        sb.Append("C");
                    }
                    if (Member.Combo > 1)
                    {
                        sb.Append("O");
                    }
                    if (Member.Combo > 2)
                    {
                        sb.Append("M");
                    }
                    if (Member.Combo > 3)
                    {
                        sb.Append("B");
                    }
                    if (Member.Combo > 4)
                    {
                        sb.Append("O");
                    }
                    if (Member.Combo == 5)
                    {
                        Member.Combo = 0;
                        sb.AppendLine("\n恭喜连续工作获得勤工奖！额外获得了500金币！");
                        Member.Cash += 500;
                    }
                }
                else
                {
                    Member.Combo = 0;
                }
                Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage(sb.ToString()));
                Member.Exp += SharedData.Instance.工资[Member.Work];
                Member.Cash += SharedData.Instance.工资[Member.Work];
                Member.Checked = DateTime.Now;
                if (Member.Exp > SharedData.Instance.需要经验值[Member.Work] && Member.Work != Enum.GetValues(typeof(Work)).Cast<Work>().Last())
                {
                    Member.Work = Member.Work.Next();
                    Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("已升级啦！接下来的工作为" + Member.Work.ToString() + ", 工资为" + SharedData.Instance.工资[Member.Work] + "金币！"));
                }
                var holiday1 = ChinaDate.GetHoliday(DateTime.Now);
                var holiday2 = ChinaDate.GetChinaHoliday(DateTime.Now);
                if (holiday1 != ChinaDate.GHoliday.无)
                {
                    Member.Cash += 2000;
                    Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("今天是" + holiday1.ToString() + "，大本营特意送了2000金币！"));
                }
                if (holiday2 != ChinaDate.NHoliday.无)
                {
                    Member.Cash += 2000;
                    Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("今天是" + holiday2.ToString() + "，大本营特意送了2000金币！"));
                }
            }
            else
            {
                Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("虽然"), new AtMessage(Member.Member.QQId), new PlainMessage("很想自主996加班，然而被雇主给轰了出去并且被告上大本营法庭处置"));
                Session.Mute(Member.Member.GroupId, Member.Member.QQId, new TimeSpan(0,1,0));
            }
            return this;
        }

        internal GameAPI Robber(ChainEventArgs e)
        {
            if (Member == null)
            {
                return this;
            }
            long tag;
            var qqs = e.MessageChain.Where(x => x is AtMessage);
            if(qqs.Count() < 1)
            {
                return this;
            }
            tag = (qqs.First() as AtMessage).Target;
            var prey = new GameAPI(e.FromGroup,tag, Session);
            if (Member.Robbed == null)
            {
                Member.Robbed = DateTime.MinValue;
            }
            if (Member.Robbed.Date == DateTime.Today)
            {
                Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("你今天已打劫过，无法继续打劫！"));
                return this;
            }
            if (prey.Member.Member.QQId == Program.LoggedInQQ || prey.Member.Member.QQId < 1 || prey.Member.Member.QQId == 2854196310)
            {
                if (prey.Member.weapon.GetType() != typeof(UltraWeapon1) && prey.Member.weapon.GetType() != typeof(UltraWeapon2))
                {
                    Random rnd = new Random();
                    Weapon weapon;
                    if (rnd.Next(10) > 5)
                    {
                        weapon = new UltraWeapon1();
                    }
                    else
                    {
                        weapon = new UltraWeapon2();
                    }
                    prey.Member.weapon = weapon;
                }
            }
            else if (prey.Member.Member.QQId == e.FromQQ)
            {
                Random rnd = new Random();
                var reduce = rnd.Next(100, 300);
                Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("把自己打了一顿，进了大本营医院，付" + reduce + "金币医药费！"));
                prey.Member.Cash -= reduce;
                prey.Dispose();
                return this;
            }
            if ((DateTime.Now - prey.Member.LastRobbed) < new TimeSpan(8, 0, 0))
            {
                Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("看见那已经被洗劫剩下个内裤的家伙，你放弃了打劫的想法"));
                prey.Dispose();
                return this;
            }
            if (prey.Member.Member.QQId == Program.LoggedInQQ || prey.Member.Member.QQId < 1 || prey.Member.Member.QQId == 2854196310)
            {
                Random rnd = new Random();
                Member.CurrentHP = Member.weapon.maxHP + Member.BonusHP;
                prey.Member.CurrentHP = prey.Member.weapon.maxHP + prey.Member.BonusHP;
                do
                {
                    var MemberDamage = rnd.Next(Member.weapon.minDamage, Member.weapon.maxDamage) + Member.BonusDamage;
                    var preyDamage = rnd.Next(prey.Member.weapon.minDamage, prey.Member.weapon.maxDamage) + prey.Member.BonusDamage;
                    Member.CurrentHP -= preyDamage;
                    prey.Member.CurrentHP -= MemberDamage;
                    if (Member.Skill != null)
                    {
                        CalculateSkill(this, prey, MemberDamage);
                    }
                    if (prey.Member.Skill != null)
                    {
                        CalculateSkill(prey, this, preyDamage);
                    }
                    if (Member.weapon.WeaponSkill != WeaponSkill.None)
                    {
                        CalculateWeaponSkill(this, prey, MemberDamage);
                    }
                    if (prey.Member.weapon.WeaponSkill != WeaponSkill.None)
                    {
                        CalculateWeaponSkill(prey, this, preyDamage);
                    }
                    Round++;
                }
                while (Member.CurrentHP > 0 && prey.Member.CurrentHP > 0);
                if (Member.CurrentHP > 0)
                {
                    var percent = rnd.Next(2, 6);
                    var get = 5000;
                    prey.Member.LastRobbed = DateTime.Now;
                    Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("恭喜"), new AtMessage(Member.Member.QQId), new PlainMessage("打劫"), new AtMessage(prey.Member.Member.QQId), new PlainMessage("成功！获得了" + get + "金币！"));
                }
                else if (Member.CurrentHP <= 0 && prey.Member.CurrentHP <= 0)
                {
                    Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("恭喜两个人都挂了，你们都在训练营里复活又跑了出来，然而双方都没有任何损失"));
                }
                else
                {
                    if (Member.Cash > 0)
                    {
                        var percent = rnd.Next(2, 6);
                        var get = (Member.Cash / 100) * percent;
                        prey.Member.Cash += get;
                        Member.Cash -= get;
                        Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("恭喜"), new AtMessage(prey.Member.Member.QQId), new PlainMessage("防御成功！还从打劫者身上获得了" + get + "金币！"));
                    }
                    else
                    {
                        Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("恭喜"), new AtMessage(prey.Member.Member.QQId), new PlainMessage("防御成功！然而由于打劫者没钱了，只能给你卖身赔钱！"));
                        Session.Mute(e.FromQQ, e.FromGroup, new TimeSpan(0, 30, 0));
                    }
                }
                Member.Robbed = DateTime.Today;
                prey.Dispose();
            }
            else if (prey.Member.Cash > 0)
            {
                Random rnd = new Random();
                Member.CurrentHP = Member.weapon.maxHP + Member.BonusHP;
                prey.Member.CurrentHP = prey.Member.weapon.maxHP + prey.Member.BonusHP;
                do
                {
                    var MemberDamage = rnd.Next(Member.weapon.minDamage, Member.weapon.maxDamage) + Member.BonusDamage;
                    var preyDamage = rnd.Next(prey.Member.weapon.minDamage, prey.Member.weapon.maxDamage) + prey.Member.BonusDamage;
                    Member.CurrentHP -= preyDamage;
                    prey.Member.CurrentHP -= MemberDamage;
                    if(Member.Skill != null)
                    {
                        CalculateSkill(this, prey, MemberDamage);
                    }
                    if(prey.Member.Skill != null)
                    {
                        CalculateSkill(prey, this, preyDamage);
                    }
                    if(Member.weapon.WeaponSkill != WeaponSkill.None)
                    {
                        CalculateWeaponSkill(this, prey, MemberDamage);
                    }
                    if(prey.Member.weapon.WeaponSkill != WeaponSkill.None)
                    {
                        CalculateWeaponSkill(prey, this, preyDamage);
                    }
                    Round++;
                }
                while (Member.CurrentHP > 0 && prey.Member.CurrentHP > 0);
                if (Member.CurrentHP > 0)
                {
                    var percent = rnd.Next(2, 6);
                    var get = (prey.Member.Cash / 100) * percent;
                    prey.Member.Cash -= get;
                    Member.Cash += get;
                    prey.Member.LastRobbed = DateTime.Now;
                    Session.SendGroupMessage(Member.Member.GroupId,  new PlainMessage("恭喜"), new AtMessage(Member.Member.QQId), new PlainMessage("打劫"), new AtMessage(prey.Member.Member.QQId), new PlainMessage("成功！获得了" + get + "金币！"));
                }
                else if (Member.CurrentHP <= 0 && prey.Member.CurrentHP <= 0)
                {
                    Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("恭喜两个人都挂了，你们都在训练营里复活又跑了出来，然而双方都没有任何损失"));
                }
                else
                {
                    if (Member.Cash > 0)
                    {
                        var percent = rnd.Next(2, 6);
                        var get = (Member.Cash / 100) * percent;
                        prey.Member.Cash += get;
                        Member.Cash -= get;
                        Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("恭喜"), new AtMessage(prey.Member.Member.QQId), new PlainMessage("防御成功！还从打劫者身上获得了" + get + "金币！"));
                    }
                    else
                    {
                        Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("恭喜"), new AtMessage(prey.Member.Member.QQId), new PlainMessage("防御成功！然而由于打劫者没钱了，只能给你卖身赔钱！"));
                        Session.Mute(e.FromQQ, e.FromGroup,  new TimeSpan(0, 30, 0));
                    }
                }
                Member.Robbed = DateTime.Today;
                prey.Dispose();
            }
            else
            {
                Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("要打劫的对象穷死了，你看见他衣服都穿不起后放弃了打劫的想法"));
            }
            return this;
        }

        internal GameAPI MemberCheck()
        {
            if (Member == null)
            {
                return this;
            }
            if (Member.weapon == null)
            {
                Member.weapon = new None();
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("的钱包现在有" + Member.Cash + "金币！\n经验值为" + Member.Exp + "点！手里的武器是" + Member.weapon.Name);
            if (Member.Skill != null)
            {
                sb.Append("\n当前技能: " + Member.Skill.Name + Member.SkillLevel + "级");
            }
            else
            {
                sb.Append("\n当前无技能");
            }
            if (Member.Checked.Date == DateTime.Today)
            {
                sb.Append("\n今天已工作");
            }
            else
            {
                sb.Append("\n今天还没工作！");
            }
            sb.AppendLine("\n当前个人仓库内拥有:");
            if(Member.Inventory == null || Member.Inventory.Count == 0 || Member.Inventory.All(x => x.ContainsCount == 0))
            {
                sb.Append("屎(掉落概率:你tm啥都没有):1");
            }
            else
            {
                foreach(var item in Member.Inventory)
                {
                    if(item.ContainsCount > 0)
                    {
                        sb.AppendLine(item.Element.Name +"(掉落概率:" + (item.Element.DropRate * 100) +"%)"+ ":" + item.ContainsCount);
                    }
                }
            }
            sb.AppendLine("\n总最高伤害值:" + (Member.BonusDamage + Member.weapon.maxDamage) + "\n总血量:" + (Member.BonusHP + Member.weapon.maxHP));
            Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId) , new PlainMessage(sb.ToString()));
            return this;
        }

        internal GameAPI Shop(ChainEventArgs e)
        {
            if (Member == null)
            {
                return this;
            }
            var weaponList = new List<Weapon>();
            foreach (Type type in Assembly.GetAssembly(typeof(Weapon)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Weapon))))
            {
                weaponList.Add((Weapon)Activator.CreateInstance(type));
            }
            weaponList.Sort((x, y) =>
            {
                if (x.Price > y.Price)
                {
                    return 1;
                }
                else if (x.Price < y.Price)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            });
            if (weaponList.Any(x => e.Message.Contains(x.Name)))
            {
                var selectedWeapon = weaponList.Where(x => e.Message.Contains(x.Name)).FirstOrDefault();
                if (selectedWeapon.Price > -1)
                {
                    if (Member.Cash >= selectedWeapon.Price)
                    {
                        Member.Cash -= selectedWeapon.Price;
                        try
                        {
                            if (Member.weapon.Price > 0)
                            {
                                Member.Cash += Member.weapon.Price / 4;
                            }
                        }
                        catch
                        {

                        }
                        Member.weapon = selectedWeapon;
                        Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("购买成功！"));
                    }
                    else
                    {
                        Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("你没有钱购买这个玩意！"));
                    }
                }
                else
                {
                    Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("你没有钱购买这个玩意！"));
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("请发送 /购买 武器名字 进行购买！");
                foreach (var w in weaponList)
                {
                    var skill = "无";
                    switch (w.WeaponSkill) 
                    {
                        case WeaponSkill.Doubled:
                            skill = "伤害递增";
                            break;
                        case WeaponSkill.KeepDamage:
                            skill = "持续伤害";
                            break;
                    }
                    if (w.Price > -1)
                    {
                        sb.AppendLine(w.Name + " 伤害：" + w.minDamage + "-" + w.maxDamage + " 血量：" + w.maxHP + " 价格：" + w.Price + " 技能：" + skill);
                    }
                }
                Session.SendGroupMessage(Member.Member.GroupId, BaseData.TextToImg(sb.ToString(), Session));
            }
            weaponList.Clear();
            return this;
        }

        internal GameAPI JackPot()
        {
            if (Member == null)
            {
                return this;
            }
            if (Member.PlayTime == null)
            {
                Member.PlayTime = DateTime.MinValue;
            }
            var nextPlay = Member.PlayTime.AddMinutes(15);
            if (DateTime.Now < nextPlay)
            {
                var wait = nextPlay - DateTime.Now;
                Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("由于你过于积极的想要赌博，被保安抓了出去，请在" + wait.Minutes + "分钟" + wait.Seconds + "秒后尝试！"));
                return this;
            }
            if (Member.Cash >= 150)
            {
                Member.PlayTime = DateTime.Now;
                Random rnd = new Random();
                int num1, num2, num3;
                if (DateTime.Now.Hour == 8 || DateTime.Now.Hour == 16)
                {
                    num1 = rnd.Next(0, 6);
                    num2 = rnd.Next(0, 6);
                    num3 = rnd.Next(0, 6);
                }
                else
                {
                    num1 = rnd.Next(0, 9);
                    num2 = rnd.Next(0, 9);
                    num3 = rnd.Next(0, 9);
                }
                if (num1 == num2 && num2 == num3)
                {
                    //Jackpot!
                    Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("恭喜获得了Jackpot!!获得了5000金币！！"));
                    Member.Cash += 5000;
                }
                else if (num1 == num2)
                {
                    Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("得到了数字|" + num1 + "|" + num2 + "|" + num3 + "|\n获取了100金币奖励！"));
                    Member.Cash += 50;
                }
                else if (num2 == num3 || num3 == num1)
                {
                    Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("得到了数字|" + num1 + "|" + num2 + "|" + num3 + "|\n获取了50金币奖励！"));
                    Member.Cash -= 50;
                }
                else
                {
                    Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("得到了数字 |" + num1 + "|" + num2 + "|" + num3 + "|"));
                    Member.Cash -= 150;
                }
                Member.Exp += 10;
            }
            else
            {
                Session.SendGroupMessage(Member.Member.GroupId, new AtMessage(Member.Member.QQId), new PlainMessage("没钱还想赌博？去去去！"));
            }
            return this;
        }

        internal GameAPI GetRank()
        {
            if (Member == null)
            {
                return this;
            }
            StringBuilder sb = new StringBuilder();
            var Members = new List<GameMember>();
            foreach (var file in Directory.GetFiles(groupPath))
            {
                //Weird error
                if (File.ReadAllText(file).Contains("</GameMember>d>"))
                {
                    File.WriteAllText(file, File.ReadAllText(file).Replace("</GameMember>d>", ""));
                }
                XmlSerializer reader = new XmlSerializer(typeof(GameMember));
                using (FileStream stream = new FileStream(file, FileMode.Open))
                {
                    try
                    {
                        Members.Add((GameMember)reader.Deserialize(stream));
                    }
                    catch(Exception ex)
                    {
                        Logger.Instance.AddLog(LogType.Error, "玩家ID" + file + "出现错误！错误资料:" + ex.Message);
                    }

                }
            }
            Members = Members.OrderByDescending(x => x.Cash).ToList();
            if (Members.Count > 10)
            {
                for (int x = 0; x < 10; x++)
                {
                    try
                    {
                        sb.AppendLine((x + 1).ToString() + ". " + Members[x].Member.Card + " 拥有 " + Members[x].Cash + " 金币，并且已拥有 " + Members[x].Exp + " 点经验值！");
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                for (int x = 0; x < Members.Count; x++)
                {
                    try
                    {
                        sb.AppendLine((x + 1).ToString() + ". " + Members[x].Member.Card + " 拥有 " + Members[x].Cash + " 金币，并且已拥有 " + Members[x].Exp + " 点经验值！");
                    }
                    catch
                    {

                    }
                }
            }
            Session.SendGroupMessage(Member.Member.GroupId, BaseData.TextToImg(sb.ToString(),Session));
            return this;
        }

        internal GameAPI SkillShop(ChainEventArgs e)
        {
            if (Member == null)
            {
                return this;
            }
            var skillList = new List<Skill>();
            foreach (Type type in Assembly.GetAssembly(typeof(Skill)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Skill))))
            {
                skillList.Add((Skill)Activator.CreateInstance(type));
            }
            if(skillList.Any(x => e.Message.Contains(x.Name))) 
            {
                var selectedSkill = skillList.Where(x => e.Message.Contains(x.Name)).FirstOrDefault();
                if (Member.Skill == null)
                {
                    //购买技能
                    var price = selectedSkill.Price[0];
                    if(Member.Cash >= price)
                    {
                        Member.Cash -= price;
                        Member.Skill = selectedSkill;
                        Member.SkillLevel = 1;
                        Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("购买技能成功！"));
                    }
                    else
                    {
                        Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("你没有钱购买这个玩意！"));
                    }
                }
                else
                {
                    //升级或者切换技能
                    if(selectedSkill.Name == Member.Skill.Name)
                    {
                        if(Member.SkillLevel == selectedSkill.Price.Length)
                        {
                            Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("当前技能等级已经是最高级！"));
                        }
                        else
                        {
                            var upgradePrice = Member.Skill.Price[Member.SkillLevel];
                            if (Member.Cash >= upgradePrice)
                            {
                                Member.SkillLevel++;
                                Member.Cash -= upgradePrice;
                                Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("技能等级已提升！现在等级为" + Member.SkillLevel));
                            }
                            else
                            {
                                Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("你没有钱升级这个玩意！"));
                            }
                        }
                    }
                    else
                    {
                        var upgradePrice = selectedSkill.Price[Member.SkillLevel - 1];
                        if (Member.Cash >= upgradePrice)
                        {
                            if(Member.SkillLevel > 1)
                            {
                                Member.SkillLevel--;
                                Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("技能更换成功！由于不熟悉新技能，自动降1级！"));
                            }
                            else
                            {
                                Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("技能更换成功！"));
                            }
                            Member.Cash -= upgradePrice;
                            Member.Skill = selectedSkill;
  
                        }
                        else
                        {
                            Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("你没有钱更换这个玩意！"));
                        }
                    }
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("请发送 /技能 技能名字 进行购买或升级 (升级无需写等级)！");
                foreach (var s in skillList)
                {
                    for(int x = 0; x < s.Amount.Length; x++)
                    {
                        sb.AppendLine(s.Name + " 等级:"+ (x + 1) + " 武器伤害倍数：" + s.Amount[x] + " 触发几率：" + (s.TriggerPercentage[x] * 100) + "% 价格：" + s.Price[x]);
                    }
                    sb.AppendLine("=========================");
                }
                Session.SendGroupMessage(Member.Member.GroupId, BaseData.TextToImg(sb.ToString(), Session));
            }
            skillList.Clear();
            return this;
        }

        internal void CalculateSkill(GameAPI Member, GameAPI prey, int MemberDamage)
        {
            Random rnd = new Random();
            var index = Member.Member.SkillLevel - 1;
            var percentage = Member.Member.Skill.TriggerPercentage[index];
            var trigger = rnd.NextDouble();
            if (trigger <= percentage)
            {
                switch (Member.Member.Skill.SkillType)
                {
                    case SkillType.DoubleHit:
                    case SkillType.Critical:
                        var extradamage = (int)Math.Round(MemberDamage * (Member.Member.Skill.Amount[index] - 1));
                        prey.Member.CurrentHP -= extradamage;
                        break;
                    case SkillType.Heal:
                        var heal = (int)Math.Round(MemberDamage * (Member.Member.Skill.Amount[index]));
                        Member.Member.CurrentHP += heal;
                        break;
                }
            }
        }
        internal void CalculateWeaponSkill(GameAPI Member, GameAPI prey, int MemberDamage)
        {
            var skill = Member.Member.weapon.WeaponSkill;
            switch (skill)
            {
                case WeaponSkill.KeepDamage:
                    var extradamage = (int)Math.Round(MemberDamage * Round / 10f);
                    if(extradamage >= Member.Member.weapon.maxDamage * 0.8)
                    {
                        extradamage = (int)Math.Round(Member.Member.weapon.maxDamage * 0.8);
                    }
                    prey.Member.CurrentHP -= extradamage;
                    break;
                case WeaponSkill.Doubled:
                    extradamage = (int)Math.Round(MemberDamage * Round / 3f);
                    if (extradamage >= (Member.Member.weapon.maxDamage * 1.2))
                    {
                        extradamage = (int)Math.Round(Member.Member.weapon.maxDamage * 1.2);
                    }
                    prey.Member.CurrentHP -= extradamage;
                    break;
            }
        }

        internal GameAPI Help()
        {
            if (Member == null)
            {
                return this;
            }
           Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("群里小游戏帮助：\n每日可/工作一次，超过一次将会被告上法庭！（禁言）\n" +
                "每日可/打劫一个群成员，只需要/打劫 @群成员 即可，不过需要注意，对方如果拥有到武器比你厉害，你可能会被反抢劫哦！\n每日可以无限/21点进行赌博，只要玩家的点数低于或等于21，并且比庄家的多就可以获胜！然而每次21点需要15分钟后才可继续进行！\n" +
                "/购买可以获得更强力武器以及预防其他人打劫你！不过注意：每次购买了新武器后旧武器将会自动以1/4价格出售哦！\n/拉霸可拼一拼看看运气，每天早上8点与晚上8点可以更高机会获得Jackpot，15分钟一次！\n/寻宝或许可以获得意外惊喜？20分钟一次！\n/合成A 指令当凑足了寻宝获得的材料后可以使用，强化伤害值，\n/合成H 则强化最高血量上限！\n/拍卖场 可查看当前群玩家贩卖的材料哦！"));
            return this;
        }

        internal GameAPI Buff(string Type)
        {
            var Buff = new Buff(this);
            if (Type == "A")
            {
               Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage(Buff.AddAttack()));
            }
            else
            {
               Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage(Buff.AddHP()));
            }
            return this;
        }

        internal GameAPI PurchaseTradeItem(string Message)
        {
            var guid = Message.Replace("/拍卖场购买", "").Trim().ToUpper();
           Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage(SharedData.Instance.merchant.PurchaseItem(this, guid, Session)));
            return this;
        }

        internal GameAPI SellTradeItem(string Message)
        {
            var arr = Message.Split(' ');
            try
            {
                var name = arr[1];
                var price = Convert.ToInt32(arr[2]);
                var amount = Convert.ToInt32(arr[3]);
                var item = elements.FirstOrDefault(x => x.Name == name);
                if(item == null)
                {
                    throw new ArgumentException();
                }
               Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage(SharedData.Instance.merchant.AddItem(item, price, amount, this)));
            }
            catch
            {
               Session.SendGroupMessage(Member.Member.GroupId, new PlainMessage("发送指令\"/拍卖场出售 材料名字 价格(纯数字) 数量(纯数字)\"进行上架想贩卖的物品(注意空格)"));
            }

            return this;
        }

        internal GameAPI ListTrade()
        {
            Session.SendGroupMessage(Member.Member.GroupId, BaseData.TextToImg(SharedData.Instance.merchant.ShowList(), Session));
            return this;
        }
    }
}
