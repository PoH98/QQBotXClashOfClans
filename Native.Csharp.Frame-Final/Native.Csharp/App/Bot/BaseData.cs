using IniParser.Model;
using IniParser;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using Funq;
using CocNET;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System.Threading;
using DataAccess;
using Native.Csharp.App.Bot.Interface;
using System.Reflection;
using System.Linq;

namespace Native.Csharp.App.Bot
{
    public class BaseData
    {
        public IniData config, thConfig;

        public readonly string[] THLevels = { "开挂玩家", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二", "十三", "十四" };
        public  Dictionary<string, string> translation { get; private set; }

        private string IPAddress;
        public long Group { get; private set; }

        public CocCore core;

        public Container container;
        public CqEventArgsBase Event { get; private set; }

        public Thread checkClanWar;

        public string LastClanWarStatus;

        public bool GameEnabled = true;

        public MutableDataTable texts;

        public readonly List<IChain> chains = new List<IChain>();
        public static BaseData Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new BaseData();
                }
                return _instance;
            }
        }

        private static BaseData _instance;

        public static void ShareEvent(CqEventArgsBase eventargs, long group)
        {
            Instance.Event = eventargs;
            Instance.Group = group;
        }

        public static void InitFirstUse()
        {
            string[] key = { "查看指令" };
            string[] val = { "@发送者\\/PlayerAPI (玩家ID) = 查询玩家资料\\/ClanAPI (部落ID) = 查询部落成员名单\\/清人 = 检查部落与群里成员谁不在部落里\\/改名 @用户 (游戏ID) = 自动获取游戏内昵称并且修改成员昵称\\/审核 (玩家ID) = 自动检查玩家科技图与已设置要求是否相符" };
            for(int x  = 0; x < key.Length; x++)
            {
                Instance.config.Sections.GetSectionData("自动回复").Keys.AddKey(key[x], val[x]);
            }
            Instance.config.Sections.GetSectionData("部落冲突").Keys.AddKey("Token","");
            Instance.config.Sections.GetSectionData("部落冲突").Keys.AddKey("Api邮箱", "");
            Instance.config.Sections.GetSectionData("部落冲突").Keys.AddKey("Api密码", "");
        }

        public static void LoadCOCData()
        {
            if (File.Exists("XGame.txt"))
            {
                Instance.GameEnabled = false;
            }
            FileIniDataParser parse = new FileIniDataParser();
            Instance.config = parse.ReadFile("config.ini", Encoding.Unicode);
            if (Instance.config != null)
            {
                Instance.translation = valuePairs(configType.兵种翻译);
                Instance.core = new CocCore(Instance.config["部落冲突"]["Token"]);
                Instance.container = Instance.core.Container;
            }
            if (File.Exists("Townhall.ini"))
            {
                Instance.thConfig = parse.ReadFile("Townhall.ini");
            }
            else
            {
                Instance.thConfig = new IniData();
                for(int x = 1; x < Instance.THLevels.Length; x++)
                {
                    Instance.thConfig.Sections.AddSection(x.ToString()+"本");
                    foreach(var key in Instance.translation.Keys)
                    {
                        Instance.thConfig[x.ToString()+"本"].AddKey(key, "99");
                    }
                }
                parse.WriteFile("Townhall.ini", Instance.thConfig,Encoding.Unicode);
            }
            try 
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebClient wc = new WebClient();
                Instance.IPAddress = wc.DownloadString("http://bot.whatismyipaddress.com/");
                wc.Dispose();
            }
            catch
            {
                Instance.IPAddress = "0.0.0.0";
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Warning, "部落冲突错误", "无法获取当前IP, 将会稍后再试！");
            }
            //Coc decrypted texts.csv
            if (File.Exists("texts.csv"))
            {
                try
                {
                    Instance.texts = DataTable.New.ReadCsv("texts.csv");
                }
                catch
                {
                    Instance.texts = null;
                }
            }

            foreach(var chain in AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(ChatCheckChain).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface))
            {
                Instance.chains.Add((IChain)Activator.CreateInstance(chain));
            }
            for(int x = 0; x < Instance.chains.Count - 1; x++)
            {
                Instance.chains[x].SetNext(Instance.chains[x + 1]);
            }
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "锁链加载", "已经加载"+ Instance.chains.Count + "锁链");
        }

        public static void SetClanID(long GroupID, string ClanID)
        {
            Instance.config["部落冲突"].AddKey(GroupID.ToString(), ClanID);
            FileIniDataParser parse = new FileIniDataParser();
            parse.WriteFile("config.ini", Instance.config, Encoding.Unicode);
        }

        public static bool CheckIP()
        {
            return (GetIP(out string newIP) == newIP);
        }

        public static void UpdateIP()
        {
            using (WebClient wc = new WebClient())
            {
                Instance.IPAddress = wc.DownloadString("http://bot.whatismyipaddress.com/");
            }
        }

        public static string GetIP(out string newIP)
        {
            using (WebClient wc = new WebClient())
            {
                newIP = wc.DownloadString("http://bot.whatismyipaddress.com/");
                return Instance.IPAddress;
            }
        }

        public static Dictionary<string, string> valuePairs(configType section)
        {
            Dictionary<string, string> temp = new Dictionary<string, string>();
            foreach(var keyvalue in Instance.config[section.ToString()])
            {
                temp.Add(keyvalue.KeyName, keyvalue.Value);
            }
            return temp;
        }

        public static Dictionary<string, int> GetTownhallTroopsLV(int thlv)
        {
            Dictionary<string, int> temp = new Dictionary<string, int>();
            foreach (var keyvalue in Instance.thConfig[thlv.ToString() + "本"])
            {
                temp.Add(keyvalue.KeyName, Convert.ToInt32(keyvalue.Value));
            }
            return temp;
        }

        public static void SetToken(string newtoken)
        {
            if(Instance.config == null)
            {
                LoadCOCData();
            }
            Instance.config["部落冲突"]["Token"] = newtoken;
            FileIniDataParser parser = new FileIniDataParser();
            parser.WriteFile("config.ini", Instance.config);
            Instance.core.Container.Dispose();
            Instance.core = null;
            Instance.core = new CocCore(Instance.config["部落冲突"]["Token"]);
            Instance.container = Instance.core.Container;
        }
    }
    public enum configType
    {
        自动回复,
        部落冲突,
        禁止词句,
        兵种翻译
    }
}
