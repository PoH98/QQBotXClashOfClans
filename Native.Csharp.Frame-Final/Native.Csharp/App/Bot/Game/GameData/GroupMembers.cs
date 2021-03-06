using Native.Csharp.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;

namespace Native.Csharp.App.GameData
{
    public class GroupMember
    {
        /// <summary>
        /// 玩家QQ资料
        /// </summary>
        public GroupMemberInfo Member { get; set; }
        /// <summary>
        /// 玩家当前钱
        /// </summary>
        public long Cash { get; set; }
        /// <summary>
        /// 玩家经验值
        /// </summary>
        public int Exp { get; set; }
        /// <summary>
        /// 玩家当前工作
        /// </summary>
        public Work Work { get; set; }
        /// <summary>
        /// 上次工作时间
        /// </summary>
        public DateTime Checked { get; set; }
        /// <summary>
        /// 连续签到次数
        /// </summary>
        public int Combo { get; set; }
        /// <summary>
        /// 武器
        /// </summary>
        public Weapon Weapon { get; set; }
        /// <summary>
        /// 当前血量
        /// </summary>
        public int CurrentHP { get; set; }
        /// <summary>
        /// 上次打劫时间
        /// </summary>
        public DateTime Robbed { get; set; }
        /// <summary>
        /// 21点等待
        /// </summary>
        public DateTime PlayTime { get; set; }
        /// <summary>
        /// 寻宝等待
        /// </summary>
        public DateTime TreasurePlayTime { get; set; }
        /// <summary>
        /// 最后一次被打劫时间
        /// </summary>
        public DateTime LastRobbed { get; set; }
        /// <summary>
        /// 个人技能
        /// </summary>
        public Skill Skill { get; set; }
        /// <summary>
        /// 个人技能等级
        /// </summary>
        public int SkillLevel { get; set; }
        /// <summary>
        /// 部落冲突的玩家标签
        /// </summary>
        public List<COCData> COCID { get; set; }
        /// <summary>
        /// 是否已经拉黑
        /// </summary>
        public bool BlackList { get; set; }
        /// <summary>
        /// 加群时间
        /// </summary>
        public DateTime Created_At { get; set; }
    }

    public class COCData
    {
        /// <summary>
        /// 部落冲突标签
        /// </summary>
        public string COCID { get; set; }
        /// <summary>
        /// 最后在部落时间
        /// </summary>
        public DateTime LastSeen { get; set; }
        /// <summary>
        /// 被机票/离开部落总天数, 回到部落就重置
        /// </summary>
        public long KickedDays { get; set; }
        /// <summary>
        /// 被机票/离开部落总天数, 回到部落不会重置
        /// </summary>
        public long TotalKickedDays { get; set; }
    }
}
