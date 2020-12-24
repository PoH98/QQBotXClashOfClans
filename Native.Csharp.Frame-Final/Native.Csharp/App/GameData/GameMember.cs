using Native.Csharp.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Native.Csharp.App.GameData
{
    public class GameMember
    {
        /// <summary>
        /// 玩家QQ资料
        /// </summary>
        public GroupMemberInfo Member;
        /// <summary>
        /// 玩家当前钱
        /// </summary>
        public long Cash;
        /// <summary>
        /// 玩家经验值
        /// </summary>
        public int Exp;
        /// <summary>
        /// 玩家当前工作
        /// </summary>
        public Work Work;
        /// <summary>
        /// 上次工作时间
        /// </summary>
        public DateTime Checked;
        /// <summary>
        /// 连续签到次数
        /// </summary>
        public int Combo;
        /// <summary>
        /// 武器
        /// </summary>
        public Weapon weapon;
        /// <summary>
        /// 当前血量
        /// </summary>
        public int CurrentHP;
        /// <summary>
        /// 上次打劫时间
        /// </summary>
        public DateTime Robbed;
        /// <summary>
        /// 21点等待10分钟
        /// </summary>
        public DateTime PlayTime;
        /// <summary>
        /// 寻宝等待10分钟
        /// </summary>
        public DateTime TreasurePlayTime;
        /// <summary>
        /// 打Boss等待时间
        /// </summary>
        public DateTime BossPlayTime;
        /// <summary>
        /// 最后一次被打劫时间
        /// </summary>
        public DateTime LastRobbed;
        /// <summary>
        /// 个人技能
        /// </summary>
        public Skill Skill;
        /// <summary>
        /// 个人技能等级
        /// </summary>
        public int SkillLevel = -1;
        /// <summary>
        /// 记录玩家ID以及最后一次他在部落的时间
        /// </summary>
        public ClanData[] ClanID_LastSeenInClan;
    }
    [Serializable]
    public class ClanData
    {
        /// <summary>
        /// 部落玩家ID
        /// </summary>
        public string ClanID;
        /// <summary>
        /// 部落玩家最后发现还在部落的时刻
        /// </summary>
        public DateTime? LastSeenInClan;
    }
}
