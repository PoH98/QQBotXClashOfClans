using Native.Csharp.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management.Instrumentation;
using System.Runtime.Serialization;

namespace Native.Csharp.App.GameData
{
    [Serializable]
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
        /// 成员上次在部落里的时间
        /// </summary>
        [OptionalField(VersionAdded = 2)]
        [DefaultValue(null)]
        public DateTime? LastSeenInClan;
        /// <summary>
        /// 寻宝等待10分钟
        /// </summary>
        [OptionalField(VersionAdded = 2)]
        public DateTime TreasurePlayTime;
        /// <summary>
        /// 打Boss等待时间
        /// </summary>
        [OptionalField(VersionAdded = 2)]
        public DateTime BossPlayTime;
        /// <summary>
        /// 上次发指令时间，用于抓机器人用
        /// </summary>
        [OptionalField(VersionAdded = 3)]
        public DateTime LastMessage;
        /// <summary>
        /// 上次发指令时间，用于抓机器人用
        /// </summary>
        [OptionalField(VersionAdded = 3)]
        public List<double> MessageRangeRecords = new List<double>();
        /// <summary>
        /// 最后一次被打劫时间
        /// </summary>
        [OptionalField(VersionAdded = 4)]
        public DateTime LastRobbed;
        /// <summary>
        /// 个人技能
        /// </summary>
        [OptionalField(VersionAdded = 5)]
        public Skill Skill;
        /// <summary>
        /// 个人技能等级
        /// </summary>
        [OptionalField(VersionAdded = 5)]
        public int SkillLevel = -1;
    }
}
