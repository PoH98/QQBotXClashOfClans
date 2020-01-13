using CocNET.Interfaces;
using System;
using System.Linq;
using System.Threading;

namespace Native.Csharp.App.Bot
{
    public class Threading
    {
        public static void CheckClanWar()
        {
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "部落冲突检测", "部落战检测系统启动");
            do
            {
                Thread.Sleep(1000);
                if (DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                {
                    ICocCoreClans clan = BaseData.Instance.container.Resolve<ICocCoreClans>();
                    try
                    {
                        foreach (var clanID in BaseData.Instance.config["部落冲突"])
                        {
                            if (clanID.KeyName.All(char.IsDigit))
                            {
                                var clanData = clan.GetCurrentWar(clanID.Value);
                                if (string.IsNullOrEmpty(clanData.Message))
                                {
                                    if (BaseData.Instance.LastClanWarStatus != clanData.State)
                                    {
                                        string status = "";
                                        switch (clanData.State)
                                        {
                                            case "inWar":
                                                status = "已开始！";
                                                break;
                                            case "warEnded":
                                                status = "已结束！";
                                                break;
                                            case "preparation":
                                                status = "已进入准备日！";
                                                break;
                                        }
                                        if (!string.IsNullOrEmpty(status))
                                        {
                                            Common.CqApi.SendGroupMessage(Convert.ToInt64(clanID.KeyName), Common.CqApi.CqCode_At(-1) + "部落战" + status);
                                        }
                                        BaseData.Instance.LastClanWarStatus = clanData.State;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
            while (Common.IsRunning);
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "部落冲突检测", "部落战检测系统停止");
        }
    }
}
