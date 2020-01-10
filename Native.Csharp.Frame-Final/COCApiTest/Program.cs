using CocNET.Interfaces;
using IniParser;
using IniParser.Model;
using Native.Csharp.App.Event;
using System;
using System.IO;
using System.Text;

namespace COCApiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                BaseData.GetNewToken();
                FileIniDataParser parse = new FileIniDataParser();
                if (!File.Exists("config.ini"))
                {
                    BaseData.config = new IniData();
                    foreach (var section in (BaseData.configType[])Enum.GetValues(typeof(BaseData.configType)))
                    {
                        BaseData.config.Sections.AddSection(section.ToString());
                    }
                    BaseData.InitFirstUse();
                    parse.WriteFile("config.ini", BaseData.config, Encoding.Unicode);
                }
                BaseData.LoadCOCData();
                var keypairs = BaseData.valuePairs(BaseData.configType.部落冲突);
                if (keypairs.ContainsKey("Clan_ID"))
                {
                    ICocCoreClans clan = BaseData.container.Resolve<ICocCoreClans>();
                    var clanData = clan.GetCurrentWar(keypairs["Clan_ID"]);
                    Console.WriteLine(clanData);
                    if (clanData == null)
                    {
                        Console.WriteLine("Clan Data is null");
                    }
                    else
                    {
                        if (clanData.State == "inWar")
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("\n你要的部落战资料：\n");
                            foreach (var member in clanData.Clan.Members)
                            {
                                if(member.Attacks == null)
                                {
                                    sb.Append(member.Name + " " + member.Tag);
                                }

                            }
                            sb.Append("战斗日结束时间：" + clanData.EndTime.ToString("dd/MM/yyyy hh:mm:ss tt"));
                            Console.WriteLine(sb.ToString());
                        }
                        else
                        {
                            Console.WriteLine(clanData.State);
                        }
                    }
                    Console.WriteLine(clanData.Message);
                }
                else
                {
                    Console.WriteLine("Clan_ID not exist");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            Console.ReadLine();
        }
    }
}
