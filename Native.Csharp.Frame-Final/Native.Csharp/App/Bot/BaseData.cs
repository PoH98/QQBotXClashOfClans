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
using Native.Csharp.Sdk.Cqp.Model;

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

        public struct LinkItem
        {
            public string Href;
            public string Text;

            public override string ToString()
            {
                return Href + "\n\t" + Text;
            }
        }

        public static class LinkFinder
        {
            public static List<LinkItem> Find(string file)
            {
                List<LinkItem> list = new List<LinkItem>();

                // 1.
                // Find all matches in file.
                MatchCollection m1 = Regex.Matches(file, @"(<a.*?>.*?</a>)",
                    RegexOptions.Singleline);

                // 2.
                // Loop over each match.
                foreach (Match m in m1)
                {
                    string value = m.Groups[1].Value;
                    LinkItem i = new LinkItem();

                    // 3.
                    // Get href attribute.
                    Match m2 = Regex.Match(value, @"href=\""(.*?)\""",
                        RegexOptions.Singleline);
                    if (m2.Success)
                    {
                        i.Href = m2.Groups[1].Value;
                    }

                    // 4.
                    // Remove inner tags from text.
                    string t = Regex.Replace(value, @"\s*<.*?>\s*", "",
                        RegexOptions.Singleline);
                    i.Text = t;

                    list.Add(i);
                }
                return list;
            }
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
