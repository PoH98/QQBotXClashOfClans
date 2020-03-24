﻿using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Native.Csharp.App.GameData;
using Native.Csharp.Sdk.Cqp;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml;
using System.Text.RegularExpressions;

namespace Native.Csharp.App.Bot
{
    public class GameAPI
    {
        #region 准备变数
        public static GameAPI Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new GameAPI();
                }
                return instance;
            }
        }
        public List<Weapon> weapon = new List<Weapon>();

        public Dictionary<long, List<GameMember>> gameMembers = new Dictionary<long, List<GameMember>>();

        public Dictionary<Work, int> 工资 = new Dictionary<Work, int> { 
            { Work.给野蛮人刷背, 100 }, 
            { Work.给弓箭手洗脚, 120 }, 
            { Work.给哥布林偷金币, 150 }, 
            { Work.给巨人当沙包, 180 }, 
            { Work.给炸弹人重组骨架, 200 }, 
            { Work.给法师搞爆炸头, 220 } ,
            { Work.给天使配眼镜, 250 },
            { Work.给龙灭火, 280 },
            { Work.给皮卡擦盔甲, 300 },
            { Work.给矿工点蜡烛, 320 },
            { Work.给雷龙充电, 350 },
            { Work.给大雪怪带孩子, 380 }
        };

        public Dictionary<Work, int> 需要经验值 = new Dictionary<Work, int> {
            { Work.给野蛮人刷背, 500 },
            { Work.给弓箭手洗脚, 1500 },
            { Work.给哥布林偷金币, 2500 },
            { Work.给巨人当沙包, 4000 },
            { Work.给炸弹人重组骨架, 6500 },
            { Work.给法师搞爆炸头, 9500 },
            { Work.给天使配眼镜, 13000 },
            { Work.给龙灭火, 18000 },
            { Work.给皮卡擦盔甲, 23000 },
            { Work.给矿工点蜡烛, 30000 },
            { Work.给雷龙充电, 36500 },
            { Work.给大雪怪带孩子, 50000 }
        };

        private string[] TreasureFindingSuccess = 
        new string[] 
        { 
            "你在寻宝路上看见了个蓝色狸猫，他给了你一个圆柱体后就跳到个圈里消失了，你获得了%G%金币！",
            "你在寻宝路上看见了个戴皇冠的人，他身上不停冒出绿色的票卷，他给了你一张绿色的卷后就消失了，你拿去卖掉后获得了%G%金币！",
            "你在森林里迷路了，在那里你看见了个小房子，房子里传出了'面对恐惧的最好办法...'，你没继续听下去就把房子炸了，并且在那里找到了%G%金币，并且发现其实回家的路就被这破房子拦住了！",
            "你到海上去寻宝，回去的路上搭了个小船，有个老妇人在你隔壁咳嗽了一声，你没在意，下船后你开始了莫名的咳嗽并且开始发烧。你获得了%G%金币！",
            "你在森林里找到了个宝箱，然而有个在打篮球的男人在你回家的时候阻拦了你，你轻松把他的篮球踢飞后赶紧溜回了家。你获得了%G%金币！",
            "你经过了一个遗迹，找到了个石制面具，你把他卖给了个黄金头发的男人，你获得了%G%金币！",
            "你刚出门，邻居的老婆婆就把你叫住，她给了你个红苹果，你看过这个故事，便把老婆婆杀死，并且拿她的苹果卖给了路边戴红帽的小女孩，你获得了%G%金币！",
            "你到海上寻宝，路上看见了个举着个火炬的女性雕像，下船后你在路上捡到个黑色的小东西，里面塞满了大量印着某个人的纸张，上面全写了100$，你把这些破纸卖给了个捡破烂的，获得了%G%金币！",
            "你出门后发现今天在下雪，你门口站着个火柴在卖女孩，你跟她买了个女孩后把女孩拿去点燃了取暖，并且在融化的雪地里找到了个小宝箱，获得了%G%金币！",
            "你在寻宝路上发现了地上充满了坑，你生气的破骂了一顿，这肯定又是某主播挖的坑！幸运的是，你在其中一个坑里挖到了些宝藏，你获得了%G%金币！"
        };

        private string[] TreasureFindingFailed = 
        new string[] 
        {
            "你到森林里发现了个巨龙，你使用了大量精力用煮菜刀把龙打死了，当你打开宝箱时箱子突然跳出了大量的杂草，你突然惊醒发现你满口的杂草，只好自讨无趣回家了！",
            "你在寻宝路上找到了个老旧的城堡，正当你想要进去的时候你看见了好几个天使在拉着个野蛮人往这里走来，你躲起来后看见天使把野蛮人拉进去城堡里，没多久就发出了惨叫，你吓得赶紧逃离了这个地方！",
            "你在路上看见了口井，你往下面丢了个金币希望可以在今天内寻到一些好的宝藏，井底回应了你的愿望，发出了呱呱呱的叫声！",
            "你在路上看见了个老巫师在点头，你跟他一起点头，到了傍晚后你觉得颈部酸痛就回家了",
            "你找到了传说中的龙王之穴，里面的龙王告诉你他知道宝藏的地点，然而他一直滔滔不绝的一直在说他以前的辉煌事迹，你听不下去只好回家了！",
            "你在路上看见个帅气的男人在不停的抖动肩膀，他似乎放出了魔法控制了你的身体，你不由自主的跟他一起抖动肩膀，直到隔天你好不容易挣脱了魔法的控制后赶紧逃回了家！",
            "你在个美好的早晨突然看见个包着头巾的男人喊着去东北玩泥巴，还说什么他大连没有家，你觉得他是疯子就只好呆在家里的被窝里瑟瑟发抖！",
            "你躺在床上玩了一天的手机，并且在一个群聊里的机器人寻找到了大量宝藏！",
            "你走到了个名叫滑尾的奇怪村庄，发现这里正在与一名奇怪金发男人发生战争，金发男人疯狂的往村子里丢各种奇怪会爆炸的纸，而滑尾村庄则疯狂的在向金发男人使用会发光的小长方形玩意反击。你为了避免被战火烧及赶紧跑回了家！",
            "你到了个小村庄，发现这里已经被强盗洗劫一空，而强盗似乎还在不远处在搬运窃取到的厕纸，你吓得赶紧跑回了家！"
        };

        private static GameAPI instance;
        #endregion
        public static void FindTreasure(CqGroupMessageEventArgs e)
        {
            var member = getMember(e);
            if (member.PlayTime == null)
            {
                member.PlayTime = DateTime.MinValue;
            }
            var nextPlay = member.PlayTime.AddMinutes(10);
            if (DateTime.Now < nextPlay)
            {
                var wait = nextPlay - DateTime.Now;
                Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "你才刚回到家又想出门，结果不小心被被窝缠住无法动弹。请在" + wait.Minutes + "分钟" + wait.Seconds + "秒后尝试！");
                return;
            }
            member.PlayTime = DateTime.Now;
            Random rnd = new Random();
            var result = rnd.Next(0, 19);
            if (result >= 10)
            {
                result -= 10;
                Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + Instance.TreasureFindingFailed[result]);
            }
            else
            {
                var coin = rnd.Next(10, 50);
                Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + Instance.TreasureFindingSuccess[result].Replace("%G%", coin.ToString()));
                member.Cash += coin;
            }
        }
        public static void GetGroupMembers(long GroupID)
        {
            if (!Instance.gameMembers.ContainsKey(GroupID))
            {
                Instance.gameMembers.Add(GroupID, new List<GameMember>());
                foreach(var mem in Common.CqApi.GetMemberList(GroupID))
                {
                    Instance.gameMembers[GroupID].Add(newMember(mem));
                }
            }
            else
            {
                var temp = Common.CqApi.GetMemberList(GroupID);
                foreach (var mem in temp)
                {
                    var memDat = Instance.gameMembers[GroupID].Where(x => x.Member.QQId == mem.QQId);
                    if (memDat.Count() < 1)
                        Instance.gameMembers[GroupID].Add(newMember(mem));
                    else if (memDat.Count() > 1)
                    {
                        foreach (var m in memDat)
                            if (m != memDat.OrderByDescending(x => x.Cash).First())
                                Instance.gameMembers[GroupID].Remove(m);
                    }
                    else
                        memDat.ToList()[0].Member.Card = mem.Card;
                }
            }
        }

        public static void MemberWork(CqGroupMessageEventArgs e)
        {
            ResolveDictionary(e.FromGroup);
            var mem = getMember(e);
            if(mem != null)
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "部落冲突Debug", mem.Checked.Date + " " + DateTime.Now.Date);
                if (mem.Checked.Date != DateTime.Now.Date)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(Common.CqApi.CqCode_At(e.FromQQ) + "已" + mem.Work.ToString() + ", 获取了" + Instance.工资[mem.Work] + "金币！");
                    if (mem.Checked.AddDays(1).Date == DateTime.Now.Date)
                    {
                        mem.Combo++;
                        if(mem.Combo > 0)
                        {
                            sb.Append("C");
                        }
                        if(mem.Combo > 1)
                        {
                            sb.Append("O");
                        }
                        if(mem.Combo > 2)
                        {
                            sb.Append("M");
                        }
                        if(mem.Combo > 3)
                        {
                            sb.Append("B");
                        }
                        if (mem.Combo > 4)
                        {
                            sb.Append("O");
                        }
                        if (mem.Combo == 5)
                        {
                            mem.Combo = 0;
                            sb.AppendLine("\n恭喜连续工作获得勤工奖！额外获得了500金币！");
                            mem.Cash += 500;
                        }
                    }
                    else
                    {
                        mem.Combo = 0;
                    }
                    Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString()); ;
                    mem.Exp += Instance.工资[mem.Work];
                    mem.Cash += Instance.工资[mem.Work];
                    mem.Checked = DateTime.Now;
                    if(mem.Exp > Instance.需要经验值[mem.Work] && mem.Work != Work.给大雪怪带孩子)
                    {
                        mem.Work = mem.Work.Next();
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "已升级啦！接下来的工作为" + mem.Work.ToString() + ", 工资为" + Instance.工资[mem.Work] + "金币！");
                    }
                }
                else
                {
                    Random rnd = new Random();
                    var cost = rnd.Next(100, 300);
                    Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "过劳，被抬入了大本营急救，丢失了工作的工资！还需要倒贴" +cost+ "的工资付医药费！");
                    mem.Cash -= cost;
                    if (mem.Cash < -1000)
                    {
                        mem.Cash = 0;
                        Common.CqApi.SetGroupBanSpeak(e.FromGroup, e.FromQQ, new TimeSpan(2,0,0));
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "因为欠债太多，已被抓去卖两个小时的屁股还钱！");
                    }
                }
            }
            else
            {
                var groupmember = Common.CqApi.GetMemberInfo(e.FromGroup, e.FromQQ);
                Instance.gameMembers[e.FromGroup].Add(newMember(groupmember));
                mem = getMember(e);
                Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "已" + mem.Work.ToString() + ", 获取了" + Instance.工资[mem.Work] + "金币！");
                mem.Exp += Instance.工资[mem.Work];
                mem.Cash += Instance.工资[mem.Work];
                mem.Checked = DateTime.Now;
            }
        }

        public static void GetRank(CqGroupMessageEventArgs e)
        {
            ResolveDictionary(e.FromGroup);
            StringBuilder sb = new StringBuilder();
            var members = Instance.gameMembers[e.FromGroup].OrderBy(x => x.Cash).Reverse().ToArray();
            if(members.Length > 10)
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
                for (int x = 0; x < members.Length; x++)
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
            Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString());
        }

        public static void SaveData()
        {
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "部落冲突游戏", "已备份群成员资料！");
            foreach (var group in Instance.gameMembers)
            {
                if (!Directory.Exists("com.coc.groupadmin\\" + group.Key))
                {
                    Directory.CreateDirectory("com.coc.groupadmin\\" + group.Key);
                }
                foreach (var member in group.Value)
                {
                    if (File.Exists("com.coc.groupadmin\\" + group.Key + "\\" + member.Member.QQId + ".bin"))
                    {
                        File.Delete("com.coc.groupadmin\\" + group.Key + "\\" + member.Member.QQId + ".bin");
                    }
                    XmlSerializer xsSubmit = new XmlSerializer(typeof(GameMember));
                    using (var sww = new StringWriter())
                    {
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent = true;
                        settings.Encoding = Encoding.Unicode;
                        using (XmlWriter writer = XmlWriter.Create(sww, settings))
                        {
                            xsSubmit.Serialize(writer, member);
                            File.WriteAllText("com.coc.groupadmin\\" + group.Key + "\\" + member.Member.QQId + ".bin", sww.ToString(), Encoding.Unicode);
                        }
                    }
                }
            }
        }



        public static void Help(CqGroupMessageEventArgs e)
        {
            Common.CqApi.SendGroupMessage(e.FromGroup, "群里小游戏帮助：\n每日可/工作一次，超过一次将会进院治疗！(扣工资)\n每日可/打劫一个群成员，只需要/打劫 @群成员 即可，不过需要注意，对方如果拥有到武器比你厉害，你可能会被反抢劫哦！\n每日可以无限/21点进行赌博，只要玩家的点数低于或等于21，并且比庄家的多就可以获胜！然而每次21点需要10分钟后才可继续进行！\n/购买可以获得更强力武器以及预防其他人打劫你！不过注意：每次购买了新武器后旧武器将会自动以半价出售哦！\n/拉霸可拼一拼看看运气，每天早上8点与晚上8点可以更高机会获得Jackpot哦！\n/寻宝或许可以获得意外惊喜？");
        }

        public static void Robber(CqGroupMessageEventArgs e)
        {
            ResolveDictionary(e.FromGroup);
            var qq = "";
            foreach (var cqCode in CqMsg.Parse(e.Message).Contents)
            {
                qq = cqCode.Dictionary["qq"];
                break;
            }
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "部落冲突Debug", "被打劫对象QQ号为" + qq);
            if (!long.TryParse(qq, out long tag))
            {
                return;
            }
            var prey = Instance.gameMembers[e.FromGroup].Where(x => x.Member.QQId == tag).FirstOrDefault();
            var me = getMember(e);
            if(me.Robbed == null)
            {
                me.Robbed = DateTime.MinValue;
            }
            if(me.Robbed.Date == DateTime.Today)
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "你今天已打劫过，无法继续打劫！");
                return;
            }
            if (prey.Member.QQId == Common.CqApi.GetLoginQQ() || prey.Member.QQId < 1 || prey.Member.QQId == 2854196310)
            {
                if (prey.weapon.GetType() != typeof(UltraWeapon1) && prey.weapon.GetType() != typeof(UltraWeapon2))
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
                    prey.weapon = weapon;
                }
                Common.CqApi.SendGroupMessage(e.FromGroup, "你尝试打劫" + prey.Member.Nick + "然而对方直接掏出了" + prey.weapon.Name + "(伤害: 3000-3600, 血量: 无限大)直接把你打成了灰，你没有损失任何金钱并且从训练营复活了，你还可以再选一次打劫对象！");
                return;
            }
            else if (prey.Member.QQId == e.FromQQ)
            {
                Random rnd = new Random();
                var reduce = rnd.Next(100, 300);
                Common.CqApi.SendGroupMessage(e.FromGroup,Common.CqApi.CqCode_At(e.FromQQ) + "把自己打了一顿，进了大本营医院，付" + reduce + "金币医药费！");
                prey.Cash -= reduce;
                return;
            }
            if (prey.Cash > 0)
            {
                Random rnd = new Random();
                me.CurrentHP = me.weapon.maxHP;
                prey.CurrentHP = prey.weapon.maxHP;
                do
                {
                    me.CurrentHP -= rnd.Next(prey.weapon.minDamage, prey.weapon.maxDamage);
                    prey.CurrentHP -= rnd.Next(me.weapon.minDamage, me.weapon.maxDamage);
                }
                while (me.CurrentHP > 0 && prey.CurrentHP > 0);
                if (me.CurrentHP > 0)
                {
                    var percent = rnd.Next(2, 6);
                    var get = (prey.Cash / 100) * percent;
                    prey.Cash -= get;
                    me.Cash += get;
                    Common.CqApi.SendGroupMessage(e.FromGroup, "恭喜" + Common.CqApi.CqCode_At(e.FromQQ) + "打劫成功！获得了" + get + "金币！");
                }
                else if (me.CurrentHP <= 0 && prey.CurrentHP <= 0)
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, "恭喜两个人都挂了，你们都在训练营里复活又跑了出来，然而双方都没有任何损失");
                }
                else
                {
                    if (me.Cash > 0)
                    {
                        var percent = rnd.Next(2, 6);
                        var get = (prey.Cash / 100) * percent;
                        prey.Cash += get;
                        me.Cash -= get;
                        Common.CqApi.SendGroupMessage(e.FromGroup, "恭喜" + Common.CqApi.CqCode_At(prey.Member.QQId) + "防御成功！还从打劫者身上获得了" + get + "金币！");
                    }
                    else
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, "恭喜" + Common.CqApi.CqCode_At(prey.Member.QQId) + "防御成功！然而由于打劫者没钱了，只能给你卖身赔钱！");
                        Common.CqApi.SetGroupBanSpeak(e.FromGroup, e.FromQQ, new TimeSpan(0, 30, 0));
                    }
                }
                me.Robbed = DateTime.Today;
            }
            else
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "要打劫的对象穷死了，你看见他衣服都穿不起后放弃了打劫的想法");
            }
        }

        public static void ReadData()
        {
            try
            {
                Instance.weapon.Clear();
                Instance.weapon.Add(new None());
                Instance.weapon.Add(new Lighting());
                Instance.weapon.Add(new Magic());
                Instance.weapon.Add(new Pekka());
                Instance.weapon.Add(new Knive());
                Instance.weapon.Add(new XBow());
                Instance.weapon.Add(new Inferno());
                Instance.weapon.Add(new UltraWeapon1());
                Instance.weapon.Add(new UltraWeapon2());
                if (!Directory.Exists("com.coc.groupadmin"))
                {
                    Directory.CreateDirectory("com.coc.groupadmin");
                    if (File.Exists("com.coc.groupadmin.bin"))
                    {
                        try
                        {
                            var fi = new FileInfo(@"com.coc.groupadmin.bin");
                            using (var binaryFile = fi.OpenRead())
                            {
                                var binaryFormatter = new BinaryFormatter();
                                var gameMembers = (Dictionary<long, List<GameMember>>)binaryFormatter.Deserialize(binaryFile);
                                foreach (var group in gameMembers)
                                {
                                    Directory.CreateDirectory("com.coc.groupadmin\\" + group.Key);
                                    foreach (var member in group.Value)
                                    {
                                        if (!File.Exists("com.coc.groupadmin\\" + group.Key + "\\" + member.Member.QQId + ".bin"))
                                        {
                                            XmlSerializer xsSubmit = new XmlSerializer(typeof(GameMember));
                                            using (var sww = new StringWriter())
                                            {
                                                using (XmlWriter writer = XmlWriter.Create(sww))
                                                {
                                                    xsSubmit.Serialize(writer, member);
                                                    File.WriteAllText("com.coc.groupadmin\\" + group.Key + "\\" + member.Member.QQId + ".bin", sww.ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "部落冲突游戏", "无法加载旧资料，已自动强制移除！");
                        }
                        finally
                        {
                            File.Delete(@"com.coc.groupadmin.bin");
                        }
                    }
                }
                else
                {
                    if(Instance.gameMembers == null)
                    {
                        Instance.gameMembers = new Dictionary<long, List<GameMember>>();
                    }
                    Instance.gameMembers.Clear();
                    foreach (var groupDirectory in Directory.GetDirectories("com.coc.groupadmin", "*", SearchOption.TopDirectoryOnly).Where(f => Regex.IsMatch(f, @"[\\/]\d+$")).ToList())
                    {
                        try
                        {
                            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "游戏加载", groupDirectory);
                            Instance.gameMembers.Add(Convert.ToInt64(groupDirectory.Split('\\').Last()), new List<GameMember>());
                            var members = Common.CqApi.GetMemberList(Convert.ToInt64(groupDirectory.Split('\\').Last()));
                            if (members == null)
                            {
                                //Maybe the group is error
                                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "群错误", "无法获取群号" + Convert.ToInt64(groupDirectory.Split('\\').Last()) + "的资料");
                                continue;
                            }
                            var savedData = Directory.GetFiles(groupDirectory, "*.bin", SearchOption.TopDirectoryOnly).ToList();
                            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "游戏加载", "QQ群成员：" + members.Count + "|已储存资料：" + savedData.Count);
                            if (savedData.Count < members.Count)
                            {
                                //Data less than group
                                foreach (var member in members)
                                {
                                    if (!savedData.Any(x => x.Contains(member.QQId.ToString())))
                                    {
                                        XmlSerializer xsSubmit = new XmlSerializer(typeof(GameMember));
                                        using (var sww = new StringWriter())
                                        {
                                            using (XmlWriter writer = XmlWriter.Create(sww))
                                            {
                                                xsSubmit.Serialize(writer, newMember(member));
                                                File.WriteAllText(Path.Combine(groupDirectory, member.QQId + ".bin"), sww.ToString());
                                            }
                                        }
                                    }
                                }
                                ReadData();
                            }
                            else if (savedData.Count > members.Count)
                            {
                                //Data more than group
                                foreach (var data in savedData)
                                {
                                    if (!members.Any(x => x.QQId.ToString() == data.Split('\\').Last().Replace(".bin","")))
                                    {
                                        Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "游戏加载", "正在删除资料！群员：" + data.Split('\\').Last());
                                        File.Delete(data);
                                    }
                                }
                                ReadData();
                            }
                            else
                            {
                                foreach (var memberDirectory in savedData)
                                {
                                    using (var stream = File.OpenRead(memberDirectory))
                                    {
                                        var serializer = new XmlSerializer(typeof(GameMember));
                                        var result = serializer.Deserialize(stream) as GameMember;
                                        Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "游戏加载", "读取资料" + result.Member.QQId + "当前钱包：" + result.Cash + "，当前经验值" + result.Exp);
                                        Instance.gameMembers[Convert.ToInt64(groupDirectory.Split('\\').Last())].Add(result);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "游戏加载", "读取资料错误！错误群号：" + groupDirectory.Split('\\').Last());
                        }
                       
                    }
                }
            }
            catch(Exception ex)
            {
                Common.CqApi.AddFatalError(ex.ToString());
            }
        }

        public static void MemberCheck(CqGroupMessageEventArgs e)
        {
            ResolveDictionary(e.FromGroup);
            var mem = getMember(e);
            if(mem != null)
            {
                if(mem.weapon == null)
                {
                    mem.weapon = new None();
                }
                string check = "";
                if (mem.Checked.Date == DateTime.Today)
                {
                    check = "已工作！";
                }
                else
                {
                    check = "还没工作！";
                }
                Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "的钱包现在有" + mem.Cash + "金币！\n经验值为" + mem.Exp + "点！手里的武器是" + mem.weapon.Name + "\n今天" + check);
            }
            else
            {
                var groupmember = Common.CqApi.GetMemberInfo(e.FromGroup, e.FromQQ);
                Instance.gameMembers[e.FromGroup].Add(newMember(groupmember));
                mem = getMember(e);
                string check = "";
                if (mem.Checked.Date == DateTime.Today)
                {
                    check = "已工作！";
                }
                else
                {
                    check = "还没工作！";
                }
                Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "的钱包现在有" + mem.Cash + "金币！\n经验值为" + mem.Exp + "点！手里的武器是" + mem.weapon.Name + "\n今天" + check);
            }
        }


        public static void Member21Point(CqGroupMessageEventArgs e)
        {
            ResolveDictionary(e.FromGroup);
            var mem = getMember(e);
            if (mem != null)
            {
                if(mem.PlayTime == null)
                {
                    mem.PlayTime = DateTime.MinValue;
                }
                var nextPlay = mem.PlayTime.AddMinutes(10);
                if (DateTime.Now < nextPlay)
                {
                    var wait = nextPlay - DateTime.Now;
                    Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "由于你过于积极的想要赌博，被保安抓了出去，请在"+ wait.Minutes +"分钟"+ wait.Seconds + "秒后尝试！");
                    return;
                }
                if (mem.Cash >= 50)
                {
                    mem.PlayTime = DateTime.Now;
                    mem.Cash -= 50;
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
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "已炸，当前点数为" + player);
                    }
                    else if (player == 21)
                    {
                        mem.Cash += 150;
                        mem.Exp += 150;
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "拿到了21点！可喜可贺！");
                        if (mem.Exp > Instance.需要经验值[mem.Work] && mem.Work != Work.给大雪怪带孩子)
                        {
                            mem.Work = mem.Work.Next();
                            Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "已升级啦！接下来的工作为" + mem.Work.ToString() + ", 工资为" + Instance.工资[mem.Work] + "金币！");
                        }
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(Common.CqApi.CqCode_At(e.FromQQ) + "当前点数为" + player + "，庄家点数为" + me + "!");
                        if (me < player || me > 21)
                        {
                            mem.Cash += 100;
                            Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString() + "恭喜玩家获胜！可喜可贺！");
                        }
                        else if(me == player)
                        {
                            mem.Cash += 50;
                            Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString() + "打成平手！");
                        }
                        else
                        {
                            Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString() + "很遗憾, 玩家败北！");
                        }
                    }
                }
                else
                {
                    var groupmember = Common.CqApi.GetMemberInfo(e.FromGroup, e.FromQQ);
                    Instance.gameMembers[e.FromGroup].Add(newMember(groupmember));
                    Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "没钱还想赌博？去去去！");
                }
            }
            else
            {
                var groupmember = Common.CqApi.GetMemberInfo(e.FromGroup, e.FromQQ);
                Instance.gameMembers[e.FromGroup].Add(newMember(groupmember));
                Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "没钱还想赌博？去去去！");
            }
        }

        public static GameMember newMember(GroupMemberInfo member)
        {
            if (member.QQId > 0 && member.QQId != Common.CqApi.GetLoginQQ())
            {
                if ((DateTime.Now - member.JoiningTime) > new TimeSpan(7, 0, 0, 0))
                {
                    return new GameMember { Member = member, Cash = 300, Exp = 300, Work = Work.给野蛮人刷背, Checked = DateTime.MinValue, Combo = 2, CurrentHP = 100, weapon = new None(), Robbed = DateTime.MinValue, PlayTime = DateTime.MinValue };
                }
                else
                {
                    return new GameMember { Member = member, Cash = 100, Exp = 100, Work = Work.给野蛮人刷背, Checked = DateTime.MinValue, Combo = 0, CurrentHP = 100, weapon = new None(), Robbed = DateTime.MinValue, PlayTime = DateTime.MinValue };
                }
            }
            else
            {
                Random rnd = new Random();
                Weapon weapon;
                if(rnd.Next(1) == 1)
                {
                    weapon = new UltraWeapon1();
                }
                else
                {
                    weapon = new UltraWeapon2();
                }
                return new GameMember { Member = member, Cash = 0, Exp = 0, Work = Work.给大雪怪带孩子, Checked = DateTime.MinValue, Combo = 0, CurrentHP = 100, weapon = weapon, Robbed = DateTime.MinValue, PlayTime = DateTime.MinValue };
            }
        }

        public static GameMember getMember(CqGroupMessageEventArgs e)
        {
            var result = Instance.gameMembers[e.FromGroup].Where(x => x.Member.QQId == e.FromQQ).FirstOrDefault(); 
            if(result == null)
            {
                GetGroupMembers(e.FromGroup);
                Instance.gameMembers[e.FromGroup].Add(newMember(Common.CqApi.GetMemberList(e.FromGroup).Where(x => x.QQId == e.FromQQ).FirstOrDefault()));
                result = Instance.gameMembers[e.FromGroup].Where(x => x.Member.QQId == e.FromQQ).OrderByDescending(x => x.Cash).FirstOrDefault();
            }
            return result;
        }

        private static void ResolveDictionary(long groupID)
        {

            if (!Instance.gameMembers.ContainsKey(groupID))
            {
                Instance.gameMembers.Add(groupID, new List<GameMember>());
                var memberlist = Common.CqApi.GetMemberList(groupID);
                foreach (var member in memberlist)
                {
                    Instance.gameMembers[groupID].Add(newMember(member));
                }
            }
            else
            {
                var members = Instance.gameMembers[groupID];
                List<GameMember> removeMember = new List<GameMember>();
                foreach (var gameMember in members)
                {
                    if(Instance.gameMembers[groupID].Where(x => x.Member.QQId == gameMember.Member.QQId).Count() > 1)
                    {
                        //Remove Duplicated
                        removeMember.Add(gameMember);
                    }
                }
                foreach(var remove in removeMember)
                {
                    Instance.gameMembers[groupID].Remove(remove);
                }
            }
        }

        public static void Shop(CqGroupMessageEventArgs e)
        {
            if(Instance.weapon.Any(x => e.Message.Contains(x.Name)))
            {
                var mem = getMember(e);
                var selectedWeapon = Instance.weapon.Where(x => e.Message.Contains(x.Name)).FirstOrDefault();
                if(selectedWeapon.Price > -1)
                {
                    if (mem.Cash >= selectedWeapon.Price)
                    {
                        mem.Cash -= selectedWeapon.Price;
                        mem.weapon = selectedWeapon;
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
                foreach(var w in Instance.weapon)
                {
                    if(w.Price > -1)
                    {
                        sb.AppendLine(w.Name + " 伤害：" + w.minDamage + "-" + w.maxDamage + " 血量：" + w.maxHP + " 价格：" + w.Price);
                    }
                }
                Common.CqApi.SendGroupMessage(e.FromGroup, sb.ToString());
            }
        }

        public static void JackPot(CqGroupMessageEventArgs e)
        {
            ResolveDictionary(e.FromGroup);
            var mem = getMember(e);
            if (mem != null)
            {
                if (mem.PlayTime == null)
                {
                    mem.PlayTime = DateTime.MinValue;
                }
                var nextPlay = mem.PlayTime.AddMinutes(10);
                if (DateTime.Now < nextPlay)
                {
                    var wait = nextPlay - DateTime.Now;
                    Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "由于你过于积极的想要赌博，被保安抓了出去，请在" + wait.Minutes + "分钟" + wait.Seconds + "秒后尝试！");
                    return;
                }
                if (mem.Cash >= 100)
                {
                    mem.PlayTime = DateTime.Now;
                    Random rnd = new Random();
                    int num1, num2, num3;
                    if (DateTime.Now.Hour == 8)
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
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "恭喜获得了Jackpot!!获得了3000金币！！");
                        mem.Cash += 3000;
                    }
                    else if (num1 == num2)
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "得到了数字|" + num1 + "|" + num2 + "|" + num3 + "|\n获取了50金币奖励！");
                        mem.Cash += 50;
                    }
                    else if (num2 == num3 || num3 == num1)
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "得到了数字|" + num1 + "|" + num2 + "|" + num3 + "|");
                        mem.Cash -= 50;
                    }
                    else
                    {
                        Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "得到了数字|" + num1 + "|" + num2 + "|" + num3 + "|");
                        mem.Cash -= 100;
                    }
                }
                else
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "没钱还想赌博？去去去！");
                }
            }
            else
            {
                var groupmember = Common.CqApi.GetMemberInfo(e.FromGroup, e.FromQQ);
                Instance.gameMembers[e.FromGroup].Add(newMember(groupmember));
                Common.CqApi.SendGroupMessage(e.FromGroup, Common.CqApi.CqCode_At(e.FromQQ) + "没钱还想赌博？去去去！");
            }
        }
    }

    public static class GameExtension
    {
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }
    }
}
