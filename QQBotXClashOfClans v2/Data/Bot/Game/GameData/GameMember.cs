using QQBotXClashOfClans_v2.GameData;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using QQBotXClashOfClans_v2;

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
        [OptionalField(VersionAdded = 1)]
        public Skill Skill;
        /// <summary>
        /// 个人技能等级
        /// </summary>
        [OptionalField(VersionAdded = 1)]
        public int SkillLevel = -1;
        /// <summary>
        /// 记录玩家ID以及最后一次他在部落的时间
        /// </summary>
        [OptionalField(VersionAdded = 2)]
        [XmlArray]
        public List<ClanData> ClanData;
        /// <summary>
        /// 玩家仓库
        /// </summary>
        [OptionalField(VersionAdded = 3)]
        public List<InventoryItem> Inventory;
        /// <summary>
        /// 额外提升伤害
        /// </summary>
        [OptionalField(VersionAdded = 3)]
        public int BonusDamage;
        /// <summary>
        /// 额外提升血量
        /// </summary>
        [OptionalField(VersionAdded = 3)]
        public int BonusHP;
    }

    [Serializable]
    public class ClanData
    {
        public ClanData()
        {

        }

        public ClanData(string ClanID, DateTime? LastSeenInClan = null)
        {
            this.ClanID = ClanID;
            this.LastSeenInClan = LastSeenInClan;
            if(LastSeenInClan == null)
            {
                InClan = true;
            }
        }
        /// <summary>
        /// 部落玩家ID
        /// </summary>
        public string ClanID;
        /// <summary>
        /// 部落玩家最后发现还在部落的时刻
        /// </summary>
        public DateTime? LastSeenInClan;
        /// <summary>
        /// 是否还在部落
        /// </summary>
        public bool InClan;
        /// <summary>
        /// 游戏内名字
        /// </summary>
        public string Name;
    }

    public class InventoryItem
    {
        /// <summary>
        /// 材料
        /// </summary>
        public Element Element;
        /// <summary>
        /// 玩家拥有数量
        /// </summary>
        public int ContainsCount;
    }
}
