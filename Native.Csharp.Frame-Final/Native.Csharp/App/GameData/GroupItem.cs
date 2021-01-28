using LiteDB;
using System.Collections.Generic;

namespace Native.Csharp.App.GameData
{
    public class GroupItem
    {
        [BsonId]
        /// <summary>
        /// 群号
        /// </summary>
        public long GroupID { get; set; }
        /// <summary>
        /// 群成员
        /// </summary>
        public List<GroupMember> Members { get; set; }
        /// <summary>
        /// 是否启动群游戏
        /// </summary>
        public bool ActiveGame { get; set; }
    }
}
