using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;

namespace Native.Csharp.App.Bot
{
    public class 服务器:ChatCheckChain
    {
        public override IEnumerable<string> GetReply(CqGroupMessageEventArgs chat)
        {
            if(chat.Message == "/服务器")
            {
                PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Privileged Time", "_Total");
                StringBuilder sb = new StringBuilder();
                sb.Append("哔波哔波？\n服务器时区：UTF-" + TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours + "\n当前时间: " + DateTime.Now.ToString() + "\n服务器CPU状态: ");
                cpuCounter.NextValue();
                Thread.Sleep(1000);
                sb.Append(Convert.ToInt32(cpuCounter.NextValue()).ToString() + "%\n");
                var wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                sb.Append("服务器内存使用: ");
                var memoryValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new
                {
                    FreePhysicalMemory = double.Parse(mo["FreePhysicalMemory"].ToString()),
                    TotalVisibleMemorySize = double.Parse(mo["TotalVisibleMemorySize"].ToString())
                }).FirstOrDefault();
                if (memoryValues != null)
                {
                    sb.Append((((memoryValues.TotalVisibleMemorySize - memoryValues.FreePhysicalMemory) / memoryValues.TotalVisibleMemorySize) * 100).ToString("0") + "%");
                }
                if(BaseData.Instance.checkClanWar == null)
                {
                    BaseData.Instance.checkClanWar = new Thread(Threading.CheckClanWar);
                    BaseData.Instance.checkClanWar.IsBackground = true;
                    BaseData.Instance.checkClanWar.Start();
                }
                sb.AppendLine("\n部落战检测线程运行状态:" + (BaseData.Instance.checkClanWar.IsAlive?"正在在线":"已断开链接，自动重启线程中..."));
                if (!BaseData.Instance.checkClanWar.IsAlive)
                {
                    BaseData.Instance.checkClanWar = new Thread(Threading.CheckClanWar);
                    BaseData.Instance.checkClanWar.IsBackground = true;
                    BaseData.Instance.checkClanWar.Start();
                }
                return new string[] { sb.ToString() };
            }
            return base.GetReply(chat);
        }
    }
}
