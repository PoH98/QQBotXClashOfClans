using Native.Csharp.App.GameData;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Native.Csharp.App.Bot
{
    public class BossFight
    {
        public static BossFight Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new BossFight();
                }
                return _instance;
            }
        }

        private static BossFight _instance;

        public Dictionary<long, Boss> boss = new Dictionary<long, Boss>();
        public static void Fight(CqGroupMessageEventArgs e)
        {
            Random rnd = new Random();
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "打Boss", "当前已储存的Boss数量(包括已经打死)"+Instance.boss.Keys.Count);
            if (Instance.boss.ContainsKey(e.FromGroup))
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "打Boss", "Boss目前血量为" + Instance.boss[e.FromGroup].HP);
                if (Instance.boss[e.FromGroup].HP < 1)
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, "现在并没有Boss出现，请多寻宝找寻Boss!");
                    return;
                }
                var lastBossHP = Instance.boss[e.FromGroup].HP;
                var member = GameAPI.Instance.gameMembers[e.FromGroup].Where(x => x.Member.QQId == e.FromQQ).First();
                if(member.BossPlayTime == DateTime.MinValue)
                {
                    member.BossPlayTime = DateTime.Now;
                }
                if(member.BossPlayTime > DateTime.Now)
                {
                    var waitTime = member.BossPlayTime - DateTime.Now;
                    Common.CqApi.SendGroupMessage(e.FromGroup, "你刚刚打Boss还没从医院醒来，请在" + Math.Round(waitTime.TotalMinutes).ToString("0") + "分钟后再试! Boss剩余血量: " + Instance.boss[e.FromGroup].HP);
                    return;
                }
                if (Instance.boss[e.FromGroup].Damage(member))
                {
                    var gain = rnd.Next(500, 600) + ((rnd.Next(5, 10) / 100) * (lastBossHP - Instance.boss[e.FromGroup].HP));
                    Common.CqApi.SendGroupMessage(e.FromGroup, "你对Boss造成了" + (Instance.boss[e.FromGroup].HP - lastBossHP) + "点伤害，成功击败Boss! 获得了" + gain + "经验值和金币！");
                    member.Cash += gain;
                    member.Exp += gain;
                }
                else
                {
                    var gain = (rnd.Next(5, 10) / 100) * lastBossHP - Instance.boss[e.FromGroup].HP;
                    Common.CqApi.SendGroupMessage(e.FromGroup, "你对Boss造成了" + (Instance.boss[e.FromGroup].HP - lastBossHP) + "点伤害, 获得了" + gain + "金币！Boss剩余血量: " + Instance.boss[e.FromGroup].HP + "\n复活时间为: " + (DateTime.Now + member.weapon.GetAwaitTime));
                    member.Cash += gain;
                }
                member.BossPlayTime = DateTime.Now + member.weapon.GetAwaitTime;
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "打Boss", "下次等待时间为" + member.BossPlayTime);
            }
            else
            {
                Common.CqApi.SendGroupMessage(e.FromGroup,"现在并没有Boss出现，请多寻宝找寻Boss!");
            }
        }
    }
}
