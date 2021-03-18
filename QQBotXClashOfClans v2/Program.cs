using IniParser;
using IniParser.Model;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QQBotXClashOfClans_v2
{
    class Program
    {
        public static long LoggedInQQ { get; private set; }
        static async Task Main(string[] args)
        {
            string key = null;
            if(args.Length > 0)
            {
                key = args[0];
            }
            long qqId = -1;
            if(args.Length > 1)
            {
                qqId = Convert.ToInt64(args[1]);
            }
            LoggedInQQ = qqId;
            try
            {
                Logger.Instance.AddLog(LogType.Info, "正在加载...");
                FileIniDataParser parse = new FileIniDataParser();
                if (!File.Exists("config.ini"))
                {
                    BaseData.Instance.config = new IniData();
                    foreach (var section in (configType[])Enum.GetValues(typeof(configType)))
                    {
                        BaseData.Instance.config.Sections.AddSection(section.ToString());
                    }
                    BaseData.InitFirstUse();
                    parse.WriteFile("config.ini", BaseData.Instance.config, Encoding.Unicode);
                }
                BaseData.LoadCOCData();
                Logger.Instance.AddLog(LogType.Info, "已加载" + BaseData.Instance.config.Sections.Count + "区域");
                if (BaseData.Instance.checkClanWar != null)
                {
                    BaseData.Instance.checkClanWar.Abort();
                    BaseData.Instance.checkClanWar = null;
                }
                BaseData.ReadGameData();
            }
            catch(Exception ex)
            {
                Logger.Instance.AddLog(LogType.Error, ex.Message);
            }
            if(key == null)
            {
                if (!BaseData.valuePairs(configType.部落冲突).TryGetValue("AuthKey", out key))
                {
                    Logger.Instance.AddLog(LogType.Error, "法找到可使用的AuthKey! 请在config.ini的部落冲突栏位添加AuthKey = 相应的AuthKey!");
                    Console.ReadLine();
                    return;
                }
            }
            if(qqId == -1)
            {
                if (!BaseData.valuePairs(configType.部落冲突).TryGetValue("QQ号", out string qqString))
                {
                    Logger.Instance.AddLog(LogType.Error, "无法找到可使用的QQ号! 请在config.ini的部落冲突栏位添加QQ号 = 相应的QQ号!");
                    Console.ReadLine();
                    return;
                }
                if(!long.TryParse(qqString, out qqId))
                {
                    Logger.Instance.AddLog(LogType.Error, "QQ号不正确！请确保QQ号只有纯数字！");
                    Console.ReadLine();
                    return;
                }
            }
            MiraiHttpSessionOptions options = new MiraiHttpSessionOptions("127.0.0.1", 8080,key);
            await using var session = new MiraiHttpSession();
            BaseData.Instance.checkClanWar = new Thread(new Threading(session).CheckClanWar)
            {
                IsBackground = true
            };
            BaseData.Instance.checkClanWar.Start();
            session.AddPlugin(new GroupMessageHandler());
            session.AddPlugin(new GroupRequestHandler());
            session.AddPlugin(new GroupExitHandler());
            Logger.Instance.AddLog(LogType.Info, "创建Event监听成功！");
            await session.ConnectAsync(options, qqId);
            Logger.Instance.AddLog(LogType.Info, "监听开始！");
            await Task.Delay(-1);
            
        }
    }
}
