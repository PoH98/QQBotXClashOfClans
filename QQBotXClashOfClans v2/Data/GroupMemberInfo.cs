using System;
using System.Collections.Generic;
using System.Text;

namespace QQBotXClashOfClans_v2
{
    [Serializable]
	public class GroupMemberInfo
	{
		/// <summary>
		/// 获取或设置一个值, 指示成员所在群
		/// </summary>
		public long GroupId { get; set; }
		/// <summary>
		/// 获取或设置一个值, 指示成员QQ
		/// </summary>
		public long QQId { get; set; }
		/// <summary>
		/// 获取或设置一个值, 指示成员昵称
		/// </summary>
		public string Nick { get; set; }
		/// <summary>
		/// 获取或设置一个值, 指示成员群名片
		/// </summary>
		public string Card { get; set; }
		/// <summary>
		/// 获取或设置一个值, 指示成员年龄
		/// </summary>
		public int Age { get; set; }
		/// <summary>
		/// 获取或设置一个值, 指示成员所在地区
		/// </summary>
		public string Area { get; set; }
	}
}
