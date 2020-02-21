﻿using Native.Csharp.Sdk.Cqp.Model;
using System;

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
    }
}
