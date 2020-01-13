using System;
using System.IO;
using IniParser;
using IniParser.Model;
using System.Text;
using Native.Csharp.App.Bot;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using Native.Csharp.Sdk.Cqp.Enum;
using System.Threading;

namespace Native.Csharp.App.Event
{
    /// <summary>
	/// Type=1003 应用被启用, 事件实现
	/// </summary>
    public class Event_CqAppEnable : ICqAppEnable
    {
        /// <summary>
		/// 处理 酷Q 的插件启动事件回调
		/// </summary>
		/// <param name="sender">事件的触发对象</param>
		/// <param name="e">事件的附加参数</param>
        public void CqAppEnable (object sender, CqAppEnableEventArgs e)
        {
            // 当应用被启用后，本方法将被调用。
            // 如果酷Q载入时应用已被启用，则在 ICqStartup 接口的实现方法被调用后，本方法也将被调用一次。
            // 如非必要，不建议在这里加载窗口。（可以添加菜单，让用户手动打开窗口）

            Common.IsRunning = true;
            FileIniDataParser parse = new FileIniDataParser();
            if (!File.Exists("config.ini"))
            {
                BaseData.Instance.config = new IniData();
                foreach(var section in (configType[])Enum.GetValues(typeof(configType)))
                {
                    BaseData.Instance.config.Sections.AddSection(section.ToString());
                }
                BaseData.InitFirstUse();
                parse.WriteFile("config.ini", BaseData.Instance.config, Encoding.Unicode);
            }
            BaseData.LoadCOCData();
            Common.CqApi.AddLoger(LogerLevel.Info, "部落冲突加载", "已加载" + BaseData.Instance.config.Sections.Count + "区域");
            if (BaseData.Instance.checkClanWar != null)
            {
                BaseData.Instance.checkClanWar.Abort();
                BaseData.Instance.checkClanWar = null;
            }
            BaseData.Instance.checkClanWar = new Thread(Threading.CheckClanWar);
            BaseData.Instance.checkClanWar.IsBackground = true;
            BaseData.Instance.checkClanWar.Start();
        }
    }
}
