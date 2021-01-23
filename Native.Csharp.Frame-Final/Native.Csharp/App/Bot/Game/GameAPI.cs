using Native.Csharp.App.GameData;
using Native.Csharp.Sdk.Cqp;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Native.Csharp.App.Bot.Game
{
    internal class GameAPI :IDisposable
    {
        internal GameMember member { get; private set; }
        private string groupPath;
        private string memberFile;
        public void Dispose()
        {
            SaveData();
        }

        internal GameAPI(CqGroupMessageEventArgs e)
        {
            groupPath = "com.coc.groupadmin\\" + e.FromGroup;
            memberFile = "com.coc.groupadmin\\" + e.FromGroup + "\\" + e.FromQQ + ".bin";
            if (!Directory.Exists(groupPath))
            {
                Directory.CreateDirectory(groupPath);
            }
            if (!File.Exists(memberFile))
            {
                var member = Common.CqApi.GetMemberInfo(e.FromGroup, e.FromQQ);
                GameMember gameMember;
                if ((DateTime.Now - member.JoiningTime) > new TimeSpan(7, 0, 0, 0))
                {
                    gameMember = new GameMember { Member = member, Cash = 300, Exp = 300, Work = Work.给野蛮人刷背, Checked = DateTime.MinValue, Combo = 2, CurrentHP = 100, weapon = new None(), Robbed = DateTime.MinValue, PlayTime = DateTime.MinValue };
                }
                else
                {
                    gameMember = new GameMember { Member = member, Cash = 100, Exp = 100, Work = Work.给野蛮人刷背, Checked = DateTime.MinValue, Combo = 0, CurrentHP = 100, weapon = new None(), Robbed = DateTime.MinValue, PlayTime = DateTime.MinValue };
                }
                var writer = new XmlSerializer(typeof(GameMember));
                using(StreamWriter stream = new StreamWriter(memberFile))
                {
                    writer.Serialize(stream, gameMember);
                }
            }
            //Weird error
            if (File.ReadAllText(memberFile).Contains("</GameMember>d>"))
            {
                File.WriteAllText(memberFile, File.ReadAllText(memberFile).Replace("</GameMember>d>", ""));
            }
            try
            {
                XmlSerializer reader = new XmlSerializer(typeof(GameMember));
                using (FileStream stream = new FileStream(memberFile, FileMode.Open))
                {
                    this.member = (GameMember)reader.Deserialize(stream);
                    this.member.Member = Common.CqApi.GetMemberInfo(e.FromGroup, e.FromQQ);
                }
            }
            catch(Exception ex)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, e.FromQQ + "资料损毁，错误资料:" + ex.Message);
            }

        }


        internal GameAPI(long group, long qqID)
        {
            groupPath = "com.coc.groupadmin\\" + group;
            memberFile = "com.coc.groupadmin\\" + group + "\\" + qqID + ".bin";
            if (!Directory.Exists(groupPath))
            {
                Directory.CreateDirectory(groupPath);
            }
            if (!File.Exists(memberFile))
            {
                var member = Common.CqApi.GetMemberInfo(group, qqID);
                GameMember gameMember;
                if ((DateTime.Now - member.JoiningTime) > new TimeSpan(7, 0, 0, 0))
                {
                    gameMember = new GameMember { Member = member, Cash = 300, Exp = 300, Work = Work.给野蛮人刷背, Checked = DateTime.MinValue, Combo = 2, CurrentHP = 100, weapon = new None(), Robbed = DateTime.MinValue, PlayTime = DateTime.MinValue };
                }
                else
                {
                    gameMember = new GameMember { Member = member, Cash = 100, Exp = 100, Work = Work.给野蛮人刷背, Checked = DateTime.MinValue, Combo = 0, CurrentHP = 100, weapon = new None(), Robbed = DateTime.MinValue, PlayTime = DateTime.MinValue };
                }
                var writer = new XmlSerializer(typeof(GameMember));
                using (StreamWriter stream = new StreamWriter(memberFile))
                {
                    writer.Serialize(stream, gameMember);
                }
            }
            else
            {
                //Weird error
                if (File.ReadAllText(memberFile).Contains("</GameMember>d>"))
                {
                    File.WriteAllText(memberFile, File.ReadAllText(memberFile).Replace("</GameMember>d>", ""));
                }
            }
            try
            {
                XmlSerializer reader = new XmlSerializer(typeof(GameMember));
                using (FileStream stream = new FileStream(memberFile, FileMode.Open))
                {
                    this.member = (GameMember)reader.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                Common.CqApi.SendGroupMessage(group, qqID + "资料损毁，错误资料:" + ex.Message);
            }
        }

        private void SaveData()
        {
            if (member == null)
            {
                return;
            }
            var writer = new XmlSerializer(typeof(GameMember));
            //Clear the contents before rewrite the data back
            File.Delete(memberFile);
            using (var wfile = new FileStream(memberFile, FileMode.CreateNew))
            {
                writer.Serialize(wfile, member);
            }
        }

        internal GameAPI Member21Point()
        {
            if (member == null)
            {
                return this;
            }
            if (member.PlayTime == null)
            {
                member.PlayTime = DateTime.MinValue;
            }
            var nextPlay = member.PlayTime.AddMinutes(15);
            if (DateTime.Now < nextPlay)
            {
                var wait = nextPlay - DateTime.Now;
                Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + "由于你过于积极的想要赌博，被保安抓了出去，请在" + wait.Minutes + "分钟" + wait.Seconds + "秒后尝试！");
                return this;
            }
            if (member.Cash >= 50)
            {
                member.PlayTime = DateTime.Now;
                member.Cash -= 50;
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
                    Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + "已炸，当前点数为" + player);
                }
                else if (player == 21)
                {
                    member.Cash += 150;
                    member.Exp += 150;
                    Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + "拿到了21点！可喜可贺！");
                    if (member.Exp > SharedData.Instance.需要经验值[member.Work] &&  member.Work != Enum.GetValues(typeof(Work)).Cast<Work>().Last())
                    {
                        member.Work = member.Work.Next();
                        Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + "已升级啦！接下来的工作为" + member.Work.ToString() + ", 工资为" + SharedData.Instance.工资[member.Work] + "金币！");
                    }
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(Common.CqApi.CqCode_At(member.Member.QQId) + "当前点数为" + player + "，庄家点数为" + me + "!");
                    if (me < player || me > 21)
                    {
                        member.Cash += 100;
                        Common.CqApi.SendGroupMessage(member.Member.GroupId, sb.ToString() + "恭喜玩家获胜！可喜可贺！");
                    }
                    else if (me == player)
                    {
                        member.Cash += 50;
                        Common.CqApi.SendGroupMessage(member.Member.GroupId, sb.ToString() + "打成平手！");
                    }
                    else
                    {
                        Common.CqApi.SendGroupMessage(member.Member.GroupId, sb.ToString() + "很遗憾, 玩家败北！");
                    }
                }
            }
            else
            {
                Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + "没钱还想赌博？去去去！");
            }
            return this;
        }

        internal GameAPI FindTreasure()
        {
            if (member == null)
            {
                return this;
            }
            if (member.TreasurePlayTime == null)
            {
                member.TreasurePlayTime = DateTime.MinValue;
            }
            var nextPlay = member.TreasurePlayTime.AddMinutes(20);
            if (DateTime.Now < nextPlay)
            {
                var wait = nextPlay - DateTime.Now;
                Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + "你才刚回到家又想出门，结果不小心被被窝缠住无法动弹。请在" + wait.Minutes + "分钟" + wait.Seconds + "秒后尝试！");
                return this;
            }
            member.TreasurePlayTime = DateTime.Now;
            Random rnd = new Random();
            var result = rnd.Next(0, (SharedData.Instance.TreasureFindingSuccess.Length + SharedData.Instance.TreasureFindingFailed.Length) - 1);
            if (result >= SharedData.Instance.TreasureFindingSuccess.Length)
            {
                result -= SharedData.Instance.TreasureFindingSuccess.Length;
                Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + SharedData.Instance.TreasureFindingFailed[result]);
            }
            else
            {
                var coin = rnd.Next(10, 50);
                Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + SharedData.Instance.TreasureFindingSuccess[result].Replace("%G%", coin.ToString()));
                member.Cash += coin;
            }
            return this;
        }

        internal GameAPI MemberWork()
        {
            if (member == null)
            {
                return this;
            }
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "部落冲突Debug", member.Checked.Date + " " + DateTime.Now.Date);
            if (member.Checked.Date != DateTime.Now.Date)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(Common.CqApi.CqCode_At(member.Member.QQId) + "已" + member.Work.ToString() + ", 获取了" + SharedData.Instance.工资[member.Work] + "金币！");
                if (member.Checked.AddDays(1).Date == DateTime.Now.Date)
                {
                    member.Combo++;
                    if (member.Combo > 0)
                    {
                        sb.Append("C");
                    }
                    if (member.Combo > 1)
                    {
                        sb.Append("O");
                    }
                    if (member.Combo > 2)
                    {
                        sb.Append("M");
                    }
                    if (member.Combo > 3)
                    {
                        sb.Append("B");
                    }
                    if (member.Combo > 4)
                    {
                        sb.Append("O");
                    }
                    if (member.Combo == 5)
                    {
                        member.Combo = 0;
                        sb.AppendLine("\n恭喜连续工作获得勤工奖！额外获得了500金币！");
                        member.Cash += 500;
                    }
                }
                else
                {
                    member.Combo = 0;
                }
                Common.CqApi.SendGroupMessage(member.Member.GroupId, sb.ToString()); ;
                member.Exp += SharedData.Instance.工资[member.Work];
                member.Cash += SharedData.Instance.工资[member.Work];
                member.Checked = DateTime.Now;
                if (member.Exp > SharedData.Instance.需要经验值[member.Work] && member.Work != Enum.GetValues(typeof(Work)).Cast<Work>().Last())
                {
                    member.Work = member.Work.Next();
                    Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + "已升级啦！接下来的工作为" + member.Work.ToString() + ", 工资为" + SharedData.Instance.工资[member.Work] + "金币！");
                }
                var holiday1 = ChinaDate.GetHoliday(DateTime.Now);
                var holiday2 = ChinaDate.GetChinaHoliday(DateTime.Now);
                if (holiday1 != ChinaDate.GHoliday.无)
                {
                    member.Cash += 2000;
                    Common.CqApi.SendGroupMessage(member.Member.GroupId, "今天是" + holiday1.ToString() + "，大本营特意送了2000金币！");
                }
                if (holiday2 != ChinaDate.NHoliday.无)
                {
                    member.Cash += 2000;
                    Common.CqApi.SendGroupMessage(member.Member.GroupId, "今天是" + holiday2.ToString() + "，大本营特意送了2000金币！");
                }
            }
            else
            {
                Common.CqApi.SendGroupMessage(member.Member.GroupId, "虽然" + Common.CqApi.CqCode_At(member.Member.QQId) + "很想自主996加班，然而被雇主给轰了出去并且被告上大本营法庭处置");
                Common.CqApi.SetGroupBanSpeak(member.Member.GroupId, member.Member.QQId, new TimeSpan(0,1,0));
            }
            return this;
        }

        internal GameAPI Robber(CqGroupMessageEventArgs e)
        {
            if (member == null)
            {
                return this;
            }
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
            var prey = new GameAPI(e.FromGroup,tag);
            if (member.Robbed == null)
            {
                member.Robbed = DateTime.MinValue;
            }
            if (member.Robbed.Date == DateTime.Today)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "你今天已打劫过，无法继续打劫！");
                return this;
            }
            if (prey.member.Member.QQId == Common.CqApi.GetLoginQQ() || prey.member.Member.QQId < 1 || prey.member.Member.QQId == 2854196310)
            {
                if (prey.member.weapon.GetType() != typeof(UltraWeapon1) && prey.member.weapon.GetType() != typeof(UltraWeapon2))
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
                    prey.member.weapon = weapon;
                }
                Common.CqApi.SendGroupMessage(e.FromGroup, "你尝试打劫" + prey.member.Member.Nick + "然而对方直接掏出了" + prey.member.weapon.Name + "(伤害: 3000-3600, 血量: 无限大)直接把你打成了灰，你没有损失任何金钱并且从训练营复活了，你还可以再选一次打劫对象！");
                prey.Dispose();
                return this;
            }
            else if (prey.member.Member.QQId == e.FromQQ)
            {
                Random rnd = new Random();
                var reduce = rnd.Next(100, 300);
                Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "把自己打了一顿，进了大本营医院，付" + reduce + "金币医药费！");
                prey.member.Cash -= reduce;
                prey.Dispose();
                return this;
            }
            if ((DateTime.Now - prey.member.LastRobbed) < new TimeSpan(8, 0, 0))
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "看见那已经被洗劫剩下个内裤的家伙，你放弃了打劫的想法");
                prey.Dispose();
                return this;
            }
            if (prey.member.Cash > 0)
            {
                Random rnd = new Random();
                member.CurrentHP = member.weapon.maxHP;
                prey.member.CurrentHP = prey.member.weapon.maxHP;
                do
                {
                    var memberDamage = rnd.Next(member.weapon.minDamage, member.weapon.maxDamage);
                    var preyDamage = rnd.Next(prey.member.weapon.minDamage, prey.member.weapon.maxDamage);
                    member.CurrentHP -= preyDamage;
                    prey.member.CurrentHP -= memberDamage;
                    if(member.Skill != null)
                    {
                        CalculateSkill(this, prey, memberDamage);
                    }
                    if(prey.member.Skill != null)
                    {
                        CalculateSkill(prey, this, preyDamage);
                    }
                }
                while (member.CurrentHP > 0 && prey.member.CurrentHP > 0);
                if (member.CurrentHP > 0)
                {
                    var percent = rnd.Next(2, 6);
                    var get = (prey.member.Cash / 100) * percent;
                    prey.member.Cash -= get;
                    member.Cash += get;
                    prey.member.LastRobbed = DateTime.Now;
                    Common.CqApi.SendGroupMessage(e.FromGroup, "恭喜" + Common.CqApi.CqCode_At(e.FromQQ) + "打劫"+Common.CqApi.CqCode_At(prey.member.Member.QQId) +"成功！获得了" + get + "金币！");
                }
                else if (member.CurrentHP <= 0 && prey.member.CurrentHP <= 0)
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, "恭喜两个人都挂了，你们都在训练营里复活又跑了出来，然而双方都没有任何损失");
                }
                else
                {
                    if (member.Cash > 0)
                    {
                        var percent = rnd.Next(2, 6);
                        var get = (member.Cash / 100) * percent;
                        prey.member.Cash += get;
                        member.Cash -= get;
                        Common.CqApi.SendGroupMessage(e.FromGroup, "恭喜" + Common.CqApi.CqCode_At(prey.member.Member.QQId) + "防御成功！还从打劫者身上获得了" + get + "金币！");
                    }
                    else
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, "恭喜" + Common.CqApi.CqCode_At(prey.member.Member.QQId) + "防御成功！然而由于打劫者没钱了，只能给你卖身赔钱！");
                        Common.CqApi.SetGroupBanSpeak(e.FromGroup, e.FromQQ, new TimeSpan(0, 30, 0));
                    }
                }
                member.Robbed = DateTime.Today;
                prey.Dispose();
            }
            else
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "要打劫的对象穷死了，你看见他衣服都穿不起后放弃了打劫的想法");
            }
            return this;
        }

        internal GameAPI MemberCheck()
        {
            if (member == null)
            {
                return this;
            }
            if (member.weapon == null)
            {
                member.weapon = new None();
            }
            string check, skill;
            if (member.Checked.Date == DateTime.Today)
            {
                check = "已工作！";
            }
            else
            {
                check = "还没工作！";
            }
            if(member.Skill != null)
            {
                skill = member.Skill.Name + member.SkillLevel +  "级";
            }
            else
            {
                skill = "无";
            }
            Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + "的钱包现在有" + member.Cash + "金币！\n经验值为" + member.Exp + "点！手里的武器是" + member.weapon.Name + "\n当前技能: " + skill + "\n今天" + check);
            return this;
        }

        internal GameAPI Shop(CqGroupMessageEventArgs e)
        {
            if (member == null)
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
                    if (member.Cash >= selectedWeapon.Price)
                    {
                        member.Cash -= selectedWeapon.Price;
                        try
                        {
                            if (member.weapon.Price > 0)
                            {
                                member.Cash += member.weapon.Price / 4;
                            }
                        }
                        catch
                        {

                        }
                        member.weapon = selectedWeapon;
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

        internal GameAPI JackPot()
        {
            if (member == null)
            {
                return this;
            }
            if (member.PlayTime == null)
            {
                member.PlayTime = DateTime.MinValue;
            }
            var nextPlay = member.PlayTime.AddMinutes(15);
            if (DateTime.Now < nextPlay)
            {
                var wait = nextPlay - DateTime.Now;
                Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + "由于你过于积极的想要赌博，被保安抓了出去，请在" + wait.Minutes + "分钟" + wait.Seconds + "秒后尝试！");
                return this;
            }
            if (member.Cash >= 150)
            {
                member.PlayTime = DateTime.Now;
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
                    Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + "恭喜获得了Jackpot!!获得了5000金币！！");
                    member.Cash += 5000;
                }
                else if (num1 == num2)
                {
                    Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + "得到了数字|" + num1 + "|" + num2 + "|" + num3 + "|\n获取了100金币奖励！");
                    member.Cash += 50;
                }
                else if (num2 == num3 || num3 == num1)
                {
                    Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + "得到了数字|" + num1 + "|" + num2 + "|" + num3 + "|\n获取了50金币奖励！");
                    member.Cash -= 50;
                }
                else
                {
                    Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + "得到了数字|" + num1 + "|" + num2 + "|" + num3 + "|");
                    member.Cash -= 150;
                }
                member.Exp += 10;
            }
            else
            {
                Common.CqApi.SendGroupMessage(member.Member.GroupId, Common.CqApi.CqCode_At(member.Member.QQId) + "没钱还想赌博？去去去！");
            }
            return this;
        }

        internal GameAPI GetRank()
        {
            if (member == null)
            {
                return this;
            }
            StringBuilder sb = new StringBuilder();
            var members = new List<GameMember>();
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
                        members.Add((GameMember)reader.Deserialize(stream));
                    }
                    catch(Exception ex)
                    {
                        Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "游戏排名", "玩家ID" + file + "出现错误！错误资料:" + ex.Message);
                    }

                }
            }
            members = members.OrderByDescending(x => x.Cash).ToList();
            if (members.Count > 10)
            {
                for (int x = 0; x < 10; x++)
                {
                    try
                    {
                        sb.AppendLine((x + 1).ToString() + ". " + members[x].Member.Card + " 拥有 " + members[x].Cash + " 金币，并且已拥有 " + members[x].Exp + " 点经验值！");
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                for (int x = 0; x < members.Count; x++)
                {
                    try
                    {
                        sb.AppendLine((x + 1).ToString() + ". " + members[x].Member.Card + " 拥有 " + members[x].Cash + " 金币，并且已拥有 " + members[x].Exp + " 点经验值！");
                    }
                    catch
                    {

                    }
                }
            }
            var r = BaseData.TextToImg(sb.ToString());
            Regex regex = new Regex(@"\[bmp:(\S*)\]");
            var match = regex.Match(r);
            var fileName = match.Groups[1].Value;
            var tempr = r.Replace(match.Groups[0].Value, "");
            Common.CqApi.SendGroupMessage(member.Member.GroupId, tempr + Common.CqApi.CqCode_Image(fileName));
            return this;
        }

        internal GameAPI SkillShop(CqGroupMessageEventArgs e)
        {
            if (member == null)
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
                if (member.Skill == null)
                {
                    //购买技能
                    var price = selectedSkill.Price[0];
                    if(member.Cash >= price)
                    {
                        member.Cash -= price;
                        member.Skill = selectedSkill;
                        member.SkillLevel = 1;
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
                    if(selectedSkill.Name == member.Skill.Name)
                    {
                        if(member.SkillLevel == selectedSkill.Price.Length)
                        {
                            Common.CqApi.SendGroupMessage(e.FromGroup, "当前技能等级已经是最高级！");
                        }
                        else
                        {
                            var upgradePrice = member.Skill.Price[member.SkillLevel];
                            if (member.Cash >= upgradePrice)
                            {
                                member.SkillLevel++;
                                member.Cash -= upgradePrice;
                                Common.CqApi.SendGroupMessage(e.FromGroup, "技能等级已提升！现在等级为" + member.SkillLevel);
                            }
                            else
                            {
                                Common.CqApi.SendGroupMessage(e.FromGroup, "你没有钱升级这个玩意！");
                            }
                        }
                    }
                    else
                    {
                        var upgradePrice = selectedSkill.Price[member.SkillLevel - 1];
                        if (member.Cash >= upgradePrice)
                        {
                            if(member.SkillLevel > 1)
                            {
                                member.SkillLevel--;
                                Common.CqApi.SendGroupMessage(e.FromGroup, "技能更换成功！由于不熟悉新技能，自动降1级！");
                            }
                            else
                            {
                                Common.CqApi.SendGroupMessage(e.FromGroup, "技能更换成功！");
                            }
                            member.Cash -= upgradePrice;
                            member.Skill = selectedSkill;
  
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
                    for(int x = 0; x < s.Amount.Length; x++)
                    {
                        sb.AppendLine(s.Name + " 等级:"+ (x + 1) + " 武器伤害倍数：" + s.Amount[x] + " 触发几率：" + (s.TriggerPercentage[x] * 100) + "% 价格：" + s.Price[x]);
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

        internal void CalculateSkill(GameAPI member, GameAPI prey, int memberDamage)
        {
            Random rnd = new Random();
            var index = member.member.SkillLevel - 1;
            var percentage = member.member.Skill.TriggerPercentage[index];
            var trigger = rnd.NextDouble();
            if (trigger <= percentage)
            {
                switch (member.member.Skill.SkillType)
                {
                    case SkillType.DoubleHit:
                    case SkillType.Critical:
                        var extradamage = (int)Math.Round(memberDamage * (member.member.Skill.Amount[index] - 1));
                        prey.member.CurrentHP -= extradamage;
                        break;
                    case SkillType.Heal:
                        var heal = (int)Math.Round(memberDamage * (member.member.Skill.Amount[index]));
                        member.member.CurrentHP += heal;
                        break;
                }
            }
        }

        internal GameAPI Help()
        {
            if (member == null)
            {
                return this;
            }
            Common.CqApi.SendGroupMessage(member.Member.GroupId, "群里小游戏帮助：\n每日可/工作一次，超过一次将会进院治疗！(扣工资)\n" +
                "每日可/打劫一个群成员，只需要/打劫 @群成员 即可，不过需要注意，对方如果拥有到武器比你厉害，你可能会被反抢劫哦！\n每日可以无限/21点进行赌博，只要玩家的点数低于或等于21，并且比庄家的多就可以获胜！然而每次21点需要15分钟后才可继续进行！\n" +
                "/购买可以获得更强力武器以及预防其他人打劫你！不过注意：每次购买了新武器后旧武器将会自动以1/4价格出售哦！\n/拉霸可拼一拼看看运气，每天早上8点与晚上8点可以更高机会获得Jackpot，15分钟一次！\n/寻宝或许可以获得意外惊喜？20分钟一次！");
            return this;
        }        
    }
}
