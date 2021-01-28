using LiteDB;
using Native.Csharp.App.GameData;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Native.Csharp.Sdk.Cqp;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Serialization;

namespace Native.Csharp.App.Bot.Game
{
    public class LiteDBGameAPI:IDisposable
    {
        private LiteDatabase db;
        private GroupItem Group;
        public GroupMember Member;
        private ILiteCollection<GroupItem> Collection;
        public LiteDBGameAPI(CqGroupMessageEventArgs e)
        {
            db = new LiteDatabase(e.FromGroup + ".db");
            db.BeginTrans();
            Collection = db.GetCollection<GroupItem>("Group");
            Collection.EnsureIndex(x => x.GroupID);
            Group = Collection.Include(x => x.Members).FindOne(x => x.GroupID == e.FromGroup);
            if(Group == null)
            {
                Collection.Insert(new GroupItem()
                {
                    GroupID = e.FromGroup,
                    ActiveGame = true,
                    Members = new List<GroupMember>()
                });
                Group = Collection.Include(x => x.Members).FindOne(x => x.GroupID == e.FromGroup);
            }
            Member = Group.Members.Where(x => x.Member.QQId == e.FromQQ).FirstOrDefault();
            if (Member == null)
            {
                Member = RegisterMember(e.FromGroup, e.FromQQ);
            }
        }

        public LiteDBGameAPI(long group, long qqID)
        {
            db = new LiteDatabase(group + ".db");
            db.BeginTrans();
            Collection = db.GetCollection<GroupItem>("Group");
            Group = Collection.Include(x => x.Members).FindOne(x => x.GroupID == group);
            if (Group == null)
            {
                Collection.Insert(new GroupItem()
                {
                    GroupID = group,
                    ActiveGame = true,
                    Members = new List<GroupMember>()
                });
                Group = Collection.Include(x => x.Members).FindOne(x => x.GroupID == group);
            }
            Member = Group.Members.Where(x => x.Member.QQId == qqID).FirstOrDefault();
            if (Member == null)
            {
                Member = RegisterMember(group, qqID);
            }
        }

        internal LiteDBGameAPI Member21Point()
        {
            if (Member.PlayTime == null)
            {
                Member.PlayTime = DateTime.MinValue;
            }
            var nextPlay = Member.PlayTime.AddMinutes(15);
            if (DateTime.Now < nextPlay)
            {
                var wait = nextPlay - DateTime.Now;
                Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + "由于你过于积极的想要赌博，被保安抓了出去，请在" + wait.Minutes + "分钟" + wait.Seconds + "秒后尝试！");
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
                    Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + "已炸，当前点数为" + player);
                }
                else if (player == 21)
                {
                    Member.Cash += 150;
                    Member.Exp += 150;
                    Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + "拿到了21点！可喜可贺！");
                    if (Member.Exp > SharedData.Instance.需要经验值[Member.Work] && Member.Work != Enum.GetValues(typeof(Work)).Cast<Work>().Last())
                    {
                        Member.Work = Member.Work.Next();
                        Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + "已升级啦！接下来的工作为" + Member.Work.ToString() + ", 工资为" + SharedData.Instance.工资[Member.Work] + "金币！");
                    }
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(Common.CqApi.CqCode_At(Member.Member.QQId) + "当前点数为" + player + "，庄家点数为" + me + "!");
                    if (me < player || me > 21)
                    {
                        Member.Cash += 100;
                        Common.CqApi.SendGroupMessage(Member.Member.GroupId, sb.ToString() + "恭喜玩家获胜！可喜可贺！");
                    }
                    else if (me == player)
                    {
                        Member.Cash += 50;
                        Common.CqApi.SendGroupMessage(Member.Member.GroupId, sb.ToString() + "打成平手！");
                    }
                    else
                    {
                        Common.CqApi.SendGroupMessage(Member.Member.GroupId, sb.ToString() + "很遗憾, 玩家败北！");
                    }
                }
            }
            else
            {
                Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + "没钱还想赌博？去去去！");
            }
            return this;
        }

        internal LiteDBGameAPI FindTreasure()
        {
            if (Member.TreasurePlayTime == null)
            {
                Member.TreasurePlayTime = DateTime.MinValue;
            }
            var nextPlay = Member.TreasurePlayTime.AddMinutes(20);
            if (DateTime.Now < nextPlay)
            {
                var wait = nextPlay - DateTime.Now;
                Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + "你才刚回到家又想出门，结果不小心被被窝缠住无法动弹。请在" + wait.Minutes + "分钟" + wait.Seconds + "秒后尝试！");
                return this;
            }
            Member.TreasurePlayTime = DateTime.Now;
            Random rnd = new Random();
            var result = rnd.Next(0, (SharedData.Instance.TreasureFindingSuccess.Length + SharedData.Instance.TreasureFindingFailed.Length) - 1);
            if (result >= SharedData.Instance.TreasureFindingSuccess.Length)
            {
                result -= SharedData.Instance.TreasureFindingSuccess.Length;
                Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + SharedData.Instance.TreasureFindingFailed[result]);
            }
            else
            {
                var coin = rnd.Next(10, 50);
                Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + SharedData.Instance.TreasureFindingSuccess[result].Replace("%G%", coin.ToString()));
                Member.Cash += coin;
            }
            return this;
        }

        internal LiteDBGameAPI MemberWork()
        {
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "部落冲突Debug", Member.Checked.Date + " " + DateTime.Now.Date);
            if (Member.Checked.Date != DateTime.Now.Date)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Common.CqApi.CqCode_At(Member.Member.QQId) + "已" + Member.Work.ToString() + ", 获取了" + SharedData.Instance.工资[Member.Work] + "金币！");
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
                Common.CqApi.SendGroupMessage(Member.Member.GroupId, sb.ToString()); ;
                Member.Exp += SharedData.Instance.工资[Member.Work];
                Member.Cash += SharedData.Instance.工资[Member.Work];
                Member.Checked = DateTime.Now;
                if (Member.Exp > SharedData.Instance.需要经验值[Member.Work] && Member.Work != Enum.GetValues(typeof(Work)).Cast<Work>().Last())
                {
                    Member.Work = Member.Work.Next();
                    Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + "已升级啦！接下来的工作为" + Member.Work.ToString() + ", 工资为" + SharedData.Instance.工资[Member.Work] + "金币！");
                }
                var holiday1 = ChinaDate.GetHoliday(DateTime.Now);
                var holiday2 = ChinaDate.GetChinaHoliday(DateTime.Now);
                if (holiday1 != ChinaDate.GHoliday.无)
                {
                    Member.Cash += 2000;
                    Common.CqApi.SendGroupMessage(Member.Member.GroupId, "今天是" + holiday1.ToString() + "，大本营特意送了2000金币！");
                }
                if (holiday2 != ChinaDate.NHoliday.无)
                {
                    Member.Cash += 2000;
                    Common.CqApi.SendGroupMessage(Member.Member.GroupId, "今天是" + holiday2.ToString() + "，大本营特意送了2000金币！");
                }
            }
            else
            {
                Common.CqApi.SendGroupMessage(Member.Member.GroupId, "虽然" + Common.CqApi.CqCode_At(Member.Member.QQId) + "很想自主996加班，然而被雇主给轰了出去并且被告上大本营法庭处置");
                Common.CqApi.SetGroupBanSpeak(Member.Member.GroupId, Member.Member.QQId, new TimeSpan(0, 1, 0));
            }
            return this;
        }

        internal LiteDBGameAPI Robber(CqGroupMessageEventArgs e)
        {
            var qq = "";
            foreach (var cqCode in CqMsg.Parse(e.Message).Contents)
            {
                qq = cqCode.Dictionary["qq"];
                break;
            }
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "部落冲突Debug", "被打劫对象QQ号为" + qq);
            if (!long.TryParse(qq, out long tag))
            {
                return this;
            }
            var prey = Group.Members.Where(x => x.Member.QQId == tag).FirstOrDefault();
            if(prey == null)
            {
                prey = RegisterMember(e.FromGroup, tag);
            }
            if (Member.Robbed == null)
            {
                Member.Robbed = DateTime.MinValue;
            }
            if (Member.Robbed.Date == DateTime.Today)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "你今天已打劫过，无法继续打劫！");
                return this;
            }
            if (prey.Member.QQId == Common.CqApi.GetLoginQQ() || prey.Member.QQId < 1 || prey.Member.QQId == 2854196310)
            {
                if (prey.Weapon.GetType() != typeof(UltraWeapon1) && prey.Weapon.GetType() != typeof(UltraWeapon2))
                {
                    Random rnd = new Random();
                    Weapon weapon;
                    if (rnd.Next(1) == 1)
                    {
                        weapon = new UltraWeapon1();
                    }
                    else
                    {
                        weapon = new UltraWeapon2();
                    }
                    prey.Weapon = weapon;
                }
                Common.CqApi.SendGroupMessage(e.FromGroup, "你尝试打劫" + prey.Member.Nick + "然而对方直接掏出了" + prey.Weapon.Name + "(伤害: 3000-3600, 血量: 无限大)直接把你打成了灰，你没有损失任何金钱并且从训练营复活了，你还可以再选一次打劫对象！");
                return this;
            }
            else if (prey.Member.QQId == e.FromQQ)
            {
                Random rnd = new Random();
                var reduce = rnd.Next(100, 300);
                Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "把自己打了一顿，进了大本营医院，付" + reduce + "金币医药费！");
                prey.Cash -= reduce;
                return this;
            }
            if ((DateTime.Now - prey.LastRobbed) < new TimeSpan(8, 0, 0))
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "看见那已经被洗劫剩下个内裤的家伙，你放弃了打劫的想法");
                return this;
            }
            if (prey.Cash > 0)
            {
                Random rnd = new Random();
                Member.CurrentHP = Member.Weapon.maxHP;
                prey.CurrentHP = prey.Weapon.maxHP;
                do
                {
                    var MemberDamage = rnd.Next(Member.Weapon.minDamage, Member.Weapon.maxDamage);
                    var preyDamage = rnd.Next(prey.Weapon.minDamage, prey.Weapon.maxDamage);
                    Member.CurrentHP -= preyDamage;
                    prey.CurrentHP -= MemberDamage;
                    if (Member.Skill != null)
                    {
                        CalculateSkill(Member, prey, MemberDamage);
                    }
                    if (prey.Skill != null)
                    {
                        CalculateSkill(prey, Member, preyDamage);
                    }
                }
                while (Member.CurrentHP > 0 && prey.CurrentHP > 0);
                if (Member.CurrentHP > 0)
                {
                    var percent = rnd.Next(2, 6);
                    var get = (prey.Cash / 100) * percent;
                    prey.Cash -= get;
                    Member.Cash += get;
                    prey.LastRobbed = DateTime.Now;
                    Common.CqApi.SendGroupMessage(e.FromGroup, "恭喜" + Common.CqApi.CqCode_At(e.FromQQ) + "打劫" + Common.CqApi.CqCode_At(prey.Member.QQId) + "成功！获得了" + get + "金币！");
                }
                else if (Member.CurrentHP <= 0 && prey.CurrentHP <= 0)
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, "恭喜两个人都挂了，你们都在训练营里复活又跑了出来，然而双方都没有任何损失");
                }
                else
                {
                    if (Member.Cash > 0)
                    {
                        var percent = rnd.Next(2, 6);
                        var get = (Member.Cash / 100) * percent;
                        prey.Cash += get;
                        Member.Cash -= get;
                        Common.CqApi.SendGroupMessage(e.FromGroup, "恭喜" + Common.CqApi.CqCode_At(prey.Member.QQId) + "防御成功！还从打劫者身上获得了" + get + "金币！");
                    }
                    else
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, "恭喜" + Common.CqApi.CqCode_At(prey.Member.QQId) + "防御成功！然而由于打劫者没钱了，只能给你卖身赔钱！");
                        Common.CqApi.SetGroupBanSpeak(e.FromGroup, e.FromQQ, new TimeSpan(0, 30, 0));
                    }
                }
                Member.Robbed = DateTime.Today;
                
            }
            else
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "要打劫的对象穷死了，你看见他衣服都穿不起后放弃了打劫的想法");
            }
            return this;
        }

        internal LiteDBGameAPI MemberCheck()
        {
            if (Member.Weapon == null)
            {
                Member.Weapon = new None();
            }
            string check, skill;
            if (Member.Checked.Date == DateTime.Today)
            {
                check = "已工作！";
            }
            else
            {
                check = "还没工作！";
            }
            if (Member.Skill != null)
            {
                skill = Member.Skill.Name + Member.SkillLevel + "级";
            }
            else
            {
                skill = "无";
            }
            Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + "的钱包现在有" + Member.Cash + "金币！\n经验值为" + Member.Exp + "点！手里的武器是" + Member.Weapon.Name + "\n当前技能: " + skill + "\n今天" + check);
            return this;
        }

        internal LiteDBGameAPI Shop(CqGroupMessageEventArgs e)
        {
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
                            if (Member.Weapon.Price > 0)
                            {
                                Member.Cash += Member.Weapon.Price / 4;
                            }
                        }
                        catch
                        {

                        }
                        Member.Weapon = selectedWeapon;
                        Common.CqApi.SendGroupMessage(e.FromGroup, "购买成功！");
                    }
                    else
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, "你没有钱购买这个玩意！");
                    }
                }
                else
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, "你没有钱购买这个玩意！");
                }
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("请发送 /购买 武器名字 进行购买！");
                foreach (var w in weaponList)
                {
                    if (w.Price > -1)
                    {
                        sb.AppendLine(w.Name + " 伤害：" + w.minDamage + "-" + w.maxDamage + " 血量：" + w.maxHP + " 价格：" + w.Price);
                    }
                }
                var r = BaseData.TextToImg(sb.ToString());
                Regex regex = new Regex(@"\[bmp:(\S*)\]");
                var match = regex.Match(r);
                var fileName = match.Groups[1].Value;
                var tempr = r.Replace(match.Groups[0].Value, "");
                Common.CqApi.SendGroupMessage(e.FromGroup, tempr + Common.CqApi.CqCode_Image(fileName));
            }
            weaponList.Clear();
            return this;
        }

        internal LiteDBGameAPI JackPot()
        {
            if (Member.PlayTime == null)
            {
                Member.PlayTime = DateTime.MinValue;
            }
            var nextPlay = Member.PlayTime.AddMinutes(15);
            if (DateTime.Now < nextPlay)
            {
                var wait = nextPlay - DateTime.Now;
                Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + "由于你过于积极的想要赌博，被保安抓了出去，请在" + wait.Minutes + "分钟" + wait.Seconds + "秒后尝试！");
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
                    Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + "恭喜获得了Jackpot!!获得了5000金币！！");
                    Member.Cash += 5000;
                }
                else if (num1 == num2)
                {
                    Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + "得到了数字|" + num1 + "|" + num2 + "|" + num3 + "|\n获取了100金币奖励！");
                    Member.Cash += 50;
                }
                else if (num2 == num3 || num3 == num1)
                {
                    Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + "得到了数字|" + num1 + "|" + num2 + "|" + num3 + "|\n获取了50金币奖励！");
                    Member.Cash -= 50;
                }
                else
                {
                    Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + "得到了数字|" + num1 + "|" + num2 + "|" + num3 + "|");
                    Member.Cash -= 150;
                }
                Member.Exp += 10;
            }
            else
            {
                Common.CqApi.SendGroupMessage(Member.Member.GroupId, Common.CqApi.CqCode_At(Member.Member.QQId) + "没钱还想赌博？去去去！");
            }
            return this;
        }

        internal LiteDBGameAPI GetRank()
        {
            StringBuilder sb = new StringBuilder();
            var rank = Group.Members.OrderByDescending(x => x.Cash).Take(10);
            foreach(var rnk in rank)
            {
                sb.AppendLine(rnk.Member.Card +" 拥有 " + rnk.Cash + " 金币，并且已拥有 " + rnk.Exp + " 经验值!");
            }
            var r = BaseData.TextToImg(sb.ToString());
            Regex regex = new Regex(@"\[bmp:(\S*)\]");
            var match = regex.Match(r);
            var fileName = match.Groups[1].Value;
            var tempr = r.Replace(match.Groups[0].Value, "");
            Common.CqApi.SendGroupMessage(Member.Member.GroupId, tempr + Common.CqApi.CqCode_Image(fileName));
            return this;
        }

        internal LiteDBGameAPI SkillShop(CqGroupMessageEventArgs e)
        {
            var skillList = new List<Skill>();
            foreach (Type type in Assembly.GetAssembly(typeof(Skill)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Skill))))
            {
                skillList.Add((Skill)Activator.CreateInstance(type));
            }
            if (skillList.Any(x => e.Message.Contains(x.Name)))
            {
                var selectedSkill = skillList.Where(x => e.Message.Contains(x.Name)).FirstOrDefault();
                if (Member.Skill == null)
                {
                    //购买技能
                    var price = selectedSkill.Price[0];
                    if (Member.Cash >= price)
                    {
                        Member.Cash -= price;
                        Member.Skill = selectedSkill;
                        Member.SkillLevel = 1;
                        Common.CqApi.SendGroupMessage(e.FromGroup, "购买技能成功！");
                    }
                    else
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, "你没有钱购买这个玩意！");
                    }
                }
                else
                {
                    //升级或者切换技能
                    if (selectedSkill.Name == Member.Skill.Name)
                    {
                        if (Member.SkillLevel == selectedSkill.Price.Length)
                        {
                            Common.CqApi.SendGroupMessage(e.FromGroup, "当前技能等级已经是最高级！");
                        }
                        else
                        {
                            var upgradePrice = Member.Skill.Price[Member.SkillLevel];
                            if (Member.Cash >= upgradePrice)
                            {
                                Member.SkillLevel++;
                                Member.Cash -= upgradePrice;
                                Common.CqApi.SendGroupMessage(e.FromGroup, "技能等级已提升！现在等级为" + Member.SkillLevel);
                            }
                            else
                            {
                                Common.CqApi.SendGroupMessage(e.FromGroup, "你没有钱升级这个玩意！");
                            }
                        }
                    }
                    else
                    {
                        var upgradePrice = selectedSkill.Price[Member.SkillLevel - 1];
                        if (Member.Cash >= upgradePrice)
                        {
                            if (Member.SkillLevel > 1)
                            {
                                Member.SkillLevel--;
                                Common.CqApi.SendGroupMessage(e.FromGroup, "技能更换成功！由于不熟悉新技能，自动降1级！");
                            }
                            else
                            {
                                Common.CqApi.SendGroupMessage(e.FromGroup, "技能更换成功！");
                            }
                            Member.Cash -= upgradePrice;
                            Member.Skill = selectedSkill;

                        }
                        else
                        {
                            Common.CqApi.SendGroupMessage(e.FromGroup, "你没有钱更换这个玩意！");
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
                    for (int x = 0; x < s.Amount.Length; x++)
                    {
                        sb.AppendLine(s.Name + " 等级:" + (x + 1) + " 武器伤害倍数：" + s.Amount[x] + " 触发几率：" + (s.TriggerPercentage[x] * 100) + "% 价格：" + s.Price[x]);
                    }
                    sb.AppendLine("=========================");
                }
                var r = BaseData.TextToImg(sb.ToString());
                Regex regex = new Regex(@"\[bmp:(\S*)\]");
                var match = regex.Match(r);
                var fileName = match.Groups[1].Value;
                var tempr = r.Replace(match.Groups[0].Value, "");
                Common.CqApi.SendGroupMessage(e.FromGroup, tempr + Common.CqApi.CqCode_Image(fileName));
            }
            skillList.Clear();
            return this;
        }

        internal void CalculateSkill(GroupMember Member, GroupMember prey, int MemberDamage)
        {
            Random rnd = new Random();
            var index = Member.SkillLevel - 1;
            var percentage = Member.Skill.TriggerPercentage[index];
            var trigger = rnd.NextDouble();
            if (trigger <= percentage)
            {
                switch (Member.Skill.SkillType)
                {
                    case SkillType.DoubleHit:
                    case SkillType.Critical:
                        var extradamage = (int)Math.Round(MemberDamage * (Member.Skill.Amount[index] - 1));
                        prey.CurrentHP -= extradamage;
                        break;
                    case SkillType.Heal:
                        var heal = (int)Math.Round(MemberDamage * (Member.Skill.Amount[index]));
                        Member.CurrentHP += heal;
                        break;
                }
            }
        }

        internal LiteDBGameAPI Help()
        {
            if (Member == null)
            {
                return this;
            }
            Common.CqApi.SendGroupMessage(Member.Member.GroupId, "群里小游戏帮助：\n每日可/工作一次，超过一次将会进院治疗！(扣工资)\n" +
                "每日可/打劫一个群成员，只需要/打劫 @群成员 即可，不过需要注意，对方如果拥有到武器比你厉害，你可能会被反抢劫哦！\n每日可以无限/21点进行赌博，只要玩家的点数低于或等于21，并且比庄家的多就可以获胜！然而每次21点需要15分钟后才可继续进行！\n" +
                "/购买可以获得更强力武器以及预防其他人打劫你！不过注意：每次购买了新武器后旧武器将会自动以1/4价格出售哦！\n/拉霸可拼一拼看看运气，每天早上8点与晚上8点可以更高机会获得Jackpot，15分钟一次！\n/寻宝或许可以获得意外惊喜？20分钟一次！");
            return this;
        }

        private GroupMember RegisterMember(long group, long qqID)
        {
            if (Directory.Exists("com.coc.groupadmin\\" + group))
            {
                if (File.Exists("com.coc.groupadmin\\" + group + "\\" + qqID + ".bin"))
                {
                    XmlSerializer reader = new XmlSerializer(typeof(GameMember));
                    using (FileStream stream = new FileStream("com.coc.groupadmin\\" + group + "\\" + qqID + ".bin", FileMode.Open))
                    {
                        var oldmember = (GameMember)reader.Deserialize(stream);
                        var obj = new GroupMember()
                        {
                            Created_At = DateTime.Now,
                            Member = Common.CqApi.GetMemberInfo(group, qqID),
                            BlackList = false,
                            Cash = oldmember.Cash,
                            Exp = oldmember.Exp,
                            Checked = oldmember.Checked,
                            Robbed = oldmember.Robbed,
                            LastRobbed = oldmember.LastRobbed,
                            Skill = oldmember.Skill,
                            Weapon = oldmember.weapon,
                            SkillLevel = oldmember.SkillLevel,
                            COCID = new List<COCData>(),
                            Combo = oldmember.Combo,
                            CurrentHP = 100,
                            PlayTime = oldmember.PlayTime,
                            TreasurePlayTime = oldmember.TreasurePlayTime,
                            Work = oldmember.Work
                        };
                        Group.Members.Add(obj);
                    }
                }
            }
            else
            {
                var obj = new GroupMember()
                {
                    Created_At = DateTime.Now,
                    Member = Common.CqApi.GetMemberInfo(group, qqID),
                    BlackList = false,
                    Cash = 100,
                    Exp = 0,
                    Checked = DateTime.MinValue,
                    Robbed = DateTime.MinValue,
                    LastRobbed = DateTime.MinValue,
                    Skill = null,
                    Weapon = new None(),
                    SkillLevel = 0,
                    COCID = new List<COCData>(),
                    Combo = 0,
                    CurrentHP = 100,
                    PlayTime = DateTime.MinValue,
                    TreasurePlayTime = DateTime.MinValue,
                    Work = Work.给野蛮人刷背
                };
                Group.Members.Add(obj);
            }
            Collection.Update(Group);
            return Group.Members.Where(x => x.Member.QQId == qqID).FirstOrDefault();
        }

        ~LiteDBGameAPI()
        {
            CleanUp();
        }
        public void Dispose()
        {
            CleanUp();
        }

        private void CleanUp()
        {
            try
            {
                Collection.Update(Group);
            }
            catch
            {

            }
            try
            {
                db.Commit();
                db.Dispose();
            }
            catch
            {

            }
        }
    }
}
