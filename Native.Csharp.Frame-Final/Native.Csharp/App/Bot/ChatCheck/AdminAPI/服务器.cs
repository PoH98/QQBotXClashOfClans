using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Native.Csharp.App.Bot
{
    public class 服务器:ChatCheckChain
    {
        private PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Privileged Time", "_Total");
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if(chat.Message == "/服务器")
            {
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
                return sb.ToString();
            }
            return base.GetReply(chat);
        }
    }
}
