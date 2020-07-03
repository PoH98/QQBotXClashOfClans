using CocNET.Types.Other;
using Native.Csharp.App.Bot;
using Native.Csharp.Sdk.Cqp;
using System;
using System.Threading;

namespace Native.Csharp.App.GameData
{
    public class Boss
    {
        public int HP = 0;
        public DateTime metTime;
        private Thread bossWaitTime;
        public Boss(long groupID)
        {
            HP = Common.CqApi.GetGroupInfo(groupID).CurrentNumber * 500;
            metTime = DateTime.Now;
            bossWaitTime = new Thread(()=>
            {
                do
                {
                    if (HP < 1)
                    {
                        return;
                    }
                    Thread.Sleep(3000);
                }
                while (DateTime.Now < metTime.AddHours(6));
                Common.CqApi.SendGroupMessage(groupID, "Boss掠夺了村庄，造成了无数的损失！大家都损失了大量金钱！");
                Random rnd = new Random();
                foreach (var member in GameAPI.Instance.gameMembers[groupID])
                {
                    var percent = rnd.Next(3, 6);
                    var lost = (member.Cash / 100) * percent;
                    member.Cash -= lost;
                }
                HP = 0;
                return;
            });
            bossWaitTime.IsBackground = true;
            bossWaitTime.Start();
        }
        /// <summary>
        /// 赢了会取得true
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool Damage(GameMember member)
        {
            var rnd = new Random();
            var memberHP = member.weapon.maxHP;
            do
            {
                var damage = rnd.Next(member.weapon.minDamage, member.weapon.maxDamage);
                HP -= damage;
                memberHP -= rnd.Next(66, 666);
            }
            while (memberHP > 0 && HP > 0);
            if(HP > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
