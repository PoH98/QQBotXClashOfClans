using IniParser.Model;
using IniParser;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Net;
using Funq;
using CocNET;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System.Threading;
using DataAccess;
using Native.Csharp.App.Bot.Interface;
using System.Linq;
using System.Drawing;
using System.Security.Cryptography;

namespace Native.Csharp.App.Bot
{
    public class BaseData
    {
        public IniData config, thConfig;

        public readonly string[] THLevels = { "开挂玩家", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十", "十一", "十二", "十三", "十四" };

        private string IPAddress;
        public long Group { get; private set; }

        public CocCore core;

        public Container container;
        public CqEventArgsBase Event { get; private set; }

        public Thread checkClanWar;

        public Dictionary<long, string> LastClanWarStatus = new Dictionary<long, string>();

        public List<long> GameEnabled = new List<long>();

        public readonly List<IChain> chains = new List<IChain>();

        public bool SplitLongText = false;

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
            string[] val = { "@发送者\\/玩家资料 (玩家ID) = 查询玩家资料\\/部落资料 (部落ID) = 查询部落成员名单\\/清人 = 检查部落与群里成员谁不在部落里\\/改名 @用户 (游戏ID) = 自动获取游戏内昵称并且修改成员昵称\\/审核 (玩家ID) = 自动检查玩家科技图与已设置要求是否相符" };
            for(int x  = 0; x < key.Length; x++)
            {
                Instance.config.Sections.GetSectionData("自动回复").Keys.AddKey(key[x], val[x]);
            }
            Instance.config.Sections.GetSectionData("部落冲突").Keys.AddKey("Token","");
            Instance.config.Sections.GetSectionData("部落冲突").Keys.AddKey("Api邮箱", "");
            Instance.config.Sections.GetSectionData("部落冲突").Keys.AddKey("Api密码", "");
            Instance.config.Sections.GetSectionData("部落冲突").Keys.AddKey("长字分段", "true");
        }

        public static void LoadCOCData()
        {
            FileIniDataParser parse = new FileIniDataParser();
            Instance.config = parse.ReadFile("config.ini", Encoding.Unicode);
            if (Instance.config != null)
            {
                Instance.core = new CocCore(Instance.config["部落冲突"]["Token"]);
                Instance.container = Instance.core.Container;
            }
            if (valuePairs(configType.部落冲突).ContainsKey("长字分段"))
            {
                bool.TryParse(valuePairs(configType.部落冲突)["长字分段"].ToLower(), out Instance.SplitLongText);
            }
            if(!Instance.config.Sections.Any(x => x.SectionName == "兵种TID"))
            {
                Instance.config.Sections.Add(new SectionData("兵种TID"));
            }
            if (File.Exists("Townhall.ini"))
            {
                Instance.thConfig = parse.ReadFile("Townhall.ini");
            }
            else
            {
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Warning, "部落冲突设置", "正在下载必要资料！");
                try
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    WebClient wc = new WebClient();
                    wc.DownloadProgressChanged += (s, ev) =>
                    {
                        Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Warning, "部落冲突设置", "正在下载必要资料！" + ev.ProgressPercentage + "%");
                    };
                    wc.DownloadFile("https://github.com/PoH98/QQBotXClashOfClans/releases/download/v1.0.3/Townhall.ini", "Townhall.ini");
                    wc.Dispose();
                }
                catch(Exception ex)
                {
                    Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Warning, "部落冲突设置", "资料下载失败！请自行手动设置Townhall.ini!" + ex.ToString());
                    Instance.thConfig = new IniData();
                    for (int x = 1; x < Instance.THLevels.Length; x++)
                    {
                        Instance.thConfig.Sections.AddSection(x.ToString() + "本");
                        foreach (var key in Instance.config["兵种翻译"])
                        {
                            Instance.thConfig[x.ToString() + "本"].AddKey(key.KeyName, "99");
                        }
                    }
                    parse.WriteFile("Townhall.ini", Instance.thConfig, Encoding.Unicode);
                }
                finally
                {
                    Instance.thConfig = parse.ReadFile("Townhall.ini");
                }
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
            foreach (var chain in AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(ChatCheckChain).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface))
            {
                Instance.chains.Add((IChain)Activator.CreateInstance(chain));
            }
            for(int x = 0; x < Instance.chains.Count - 1; x++)
            {
                Instance.chains[x].SetNext(Instance.chains[x + 1]);
            }
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "锁链加载", "已经加载"+ Instance.chains.Count + "锁链");
        }

        public static void UpdateTranslate(FileIniDataParser parse)
        {
            //Coc decrypted texts.csv
            if (File.Exists("texts.csv"))
            {
                try
                {
                    var buffer = DataTable.New.ReadCsv("texts.csv");
                    foreach (var keyval in Instance.config["兵种翻译"])
                    {
                        var translate = buffer.Rows.Where(x => x["EN"].ToString().Replace(" ","_") == keyval.KeyName);
                        if (translate.Count() > 0)
                        {
                            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "翻译加载", "加载成功" + keyval.KeyName + "=" + translate.First()["CN"]);
                            Instance.config["兵种翻译"][keyval.KeyName] = translate.First()["CN"];
                            if (!Instance.config["兵种TID"].ContainsKey(translate.First()["TID"]))
                                Instance.config["兵种TID"].AddKey(translate.First()["TID"]);
                            Instance.config["兵种TID"][translate.First()["TID"]] = translate.First()["EN"].Replace(" ", "_");
                        }
                    }
                    Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "翻译加载", "已成功覆盖config.ini翻译");
                    parse.WriteFile("config.ini", Instance.config, Encoding.Unicode);
                    buffer.KeepColumns(false,"");
                    GC.Collect();
                }
                catch
                {

                }
            }
        }

        public static void UpdateTownhallINI(FileIniDataParser parse)
        {
            if(File.Exists("characters.csv") && File.Exists("buildings.csv") && File.Exists("spells.csv") && File.Exists("heroes.csv"))
            {
                var iniConfig = new IniData();
                for(int x = 0; x < 15; x++)
                {
                    iniConfig.Sections.Add(new SectionData(x + "本"));
                }
                var characters = DataTable.New.ReadCsv("characters.csv");
                characters.KeepColumns("Name", "TroopLevel", "LaboratoryLevel", "TID", "ProductionBuilding", "BarrackLevel");
                var buildings = DataTable.New.ReadCsv("buildings.csv");
                buildings.KeepColumns("Name", "TownHallLevel");
                var heroes = DataTable.New.ReadCsv("heroes.csv");
                heroes.KeepColumns("Name","TID", "UpgradeResource", "RequiredTownHallLevel");
                var spells = DataTable.New.ReadCsv("spells.csv");
                List<int> LabTHLvl = new List<int>();
                string Name = null, barrack =  null;
                foreach (var build in buildings.Rows)
                {
                    if (build["Name"] == "Laboratory")
                    {
                        //lab starts
                        Name = "Laboratory";
                        LabTHLvl.Add(Convert.ToInt32(build["TownHallLevel"]));
                    }
                    else if (Name == "Laboratory" && string.IsNullOrEmpty(build["Name"]))
                    {
                        //lab level
                        LabTHLvl.Add(Convert.ToInt32(build["TownHallLevel"]));
                    }
                    else if(Name != null && !string.IsNullOrEmpty(build["Name"]))
                    {
                        //not flag anymore
                        Name = null;
                        break;
                    }
                }
                buildings.KeepColumns(false, "");
                foreach(var row in characters.Rows)
                {
                    if(row["Name"] == "String")
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(row["Name"]))
                    {
                        Name = row["TID"];
                        barrack = row["ProductionBuilding"];
                    }
                    try
                    {
                        if(barrack != "Barrack" && barrack != "Dark Elixir Barrack" && barrack != "SiegeWorkshop")
                        {
                            continue;
                        }
                        var tlevel = row["TroopLevel"];
                        var llevel = Convert.ToInt32(row["LaboratoryLevel"]);
                        var thlevel = LabTHLvl[llevel - 1];
                        var key = Instance.config["兵种TID"][Name];
                        for(int x = 0; x < 15; x++)
                        {
                            if(!iniConfig[x + "本"].ContainsKey(key))
                            {
                                iniConfig[x + "本"].AddKey(key);
                                iniConfig[x + "本"][key] = "0";
                            }
                        }
                        for(int x = thlevel; x < 15; x++)
                        {
                            var lv = Convert.ToInt32(iniConfig[x + "本"][key]);
                            var newlv = Convert.ToInt32(tlevel);
                            if (newlv > lv)
                                iniConfig[x + "本"][key] = tlevel;
                            else
                                break;
                        }
                    }
                    catch
                    {
                        
                    }

                }
                characters.KeepColumns(false, "");
                int lvloop = 1;
                foreach(var row in heroes.Rows)
                {
                    if (row["Name"] == "String")
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(row["Name"]))
                    {
                        Name = row["TID"];
                        barrack = row["UpgradeResource"];
                        lvloop = 1;
                    }
                    if (barrack == "Elixir2")
                    {
                        continue;
                    }

                    try
                    {
                        var key = Instance.config["兵种TID"][Name];
                        var tlevel = Convert.ToInt32(row["RequiredTownHallLevel"]);
                        for (int x = 0; x < 15; x++)
                        {
                            if (!iniConfig[x + "本"].ContainsKey(key))
                            {
                                iniConfig[x + "本"].AddKey(key);
                                iniConfig[x + "本"][key] = "0";
                            }
                        }
                        iniConfig[tlevel + "本"][key] = lvloop.ToString();
                        lvloop++;
                    }
                    catch
                    {

                    }
                }
                heroes.KeepColumns(false, "");
                foreach(var row in spells.Rows)
                {
                    if (row["Name"] == "String")
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(row["Name"]))
                    {
                        Name = row["TID"];
                        lvloop = 1;
                    }
                    try
                    {
                        var llevel = Convert.ToInt32(row["LaboratoryLevel"]);
                        var thlevel = LabTHLvl[llevel - 1];
                        var key = Instance.config["兵种TID"][Name];
                        for (int x = 0; x < 15; x++)
                        {
                            if (!iniConfig[x + "本"].ContainsKey(key))
                            {
                                iniConfig[x + "本"].AddKey(key);
                                iniConfig[x + "本"][key] = "0";
                            }
                        }
                        for (int x = thlevel; x < 15; x++)
                        {
                            var lv = Convert.ToInt32(iniConfig[x + "本"][key]);
                            var newlv = lvloop;
                            if (newlv > lv)
                                iniConfig[x + "本"][key] = lvloop.ToString();
                            else
                                break;
                        }
                        lvloop++;
                    }
                    catch
                    {

                    }
                }
                spells.KeepColumns(false, "");
                if (File.Exists("Townhall.ini"))
                {
                    File.Delete("Townhall.ini");
                }
                Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "等级加载", "已成功覆盖Townhall.ini设置");
                parse.WriteFile("Townhall.ini", iniConfig, Encoding.Unicode);
                GC.Collect();
            }
        }

        public static void ReadGameData()
        {
            if (File.Exists("XGame.txt"))
            {
                var lines = File.ReadAllLines("XGame.txt");
                foreach (var line in lines)
                {
                    if (long.TryParse(line, out long groupNum))
                    {
                        Instance.GameEnabled.Add(groupNum);
                    }
                }
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

        public static string TextToImg(string text)
        {
            string md5 = MD5Hash(text);
            string rndName = Path.Combine("Buffer\\" + md5);
            if (!File.Exists(rndName))
            {
                Convert_Text_to_Image(text, "Times New Roman", 13).Save(rndName);
            }
            return rndName;
        }
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        private static Bitmap Convert_Text_to_Image(string txt, string fontname, int fontsize)
        {
            //creating bitmap image
            Bitmap bmp = new Bitmap(1, 1);

            //FromImage method creates a new Graphics from the specified Image.
            Graphics graphics = Graphics.FromImage(bmp);
            // Create the Font object for the image text drawing.
            Font font = new Font(fontname, fontsize);
            // Instantiating object of Bitmap image again with the correct size for the text and font.
            SizeF stringSize = graphics.MeasureString(txt, font);
            bmp = new Bitmap(bmp, (int)stringSize.Width, (int)stringSize.Height);
            graphics = Graphics.FromImage(bmp);

            /* It can also be a way
           bmp = new Bitmap(bmp, new Size((int)graphics.MeasureString(txt, font).Width, (int)graphics.MeasureString(txt, font).Height));*/

            //Draw Specified text with specified format 
            graphics.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, (float)stringSize.Width, (float)stringSize.Height));
            graphics.DrawString(txt, font, Brushes.White, 0, 0);
            font.Dispose();
            graphics.Flush();
            graphics.Dispose();
            return bmp;     //return Bitmap Image 
        }
    }
    public enum configType
    {
        自动回复,
        部落冲突,
        禁止词句,
        兵种翻译,
        兵种TID
    }
}
