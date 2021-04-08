﻿using Mirai_CSharp.Models;
using QQBotXClashOfClans_v2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QQBotXClashOfClans_v2
{
    public class 服务器:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if(chat.Message == "/服务器")
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("哔波哔波？\n服务器时区：UTF-" + TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours + "\n当前时间: " + DateTime.Now.ToString() + "\n服务器CPU状态: ");
                sb.Append(Convert.ToInt32(BaseData.Instance.cpuUsage).ToString("N0") + "%\n");
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
                    Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                    Environment.Exit(0);
                }
                sb.AppendLine("\n部落战检测线程运行状态:" + (BaseData.Instance.checkClanWar.IsAlive?"正在在线":"已断开链接，自动重启线程中..."));
                if (!BaseData.Instance.checkClanWar.IsAlive)
                {
                    Thread t = new Thread(() =>
                    {
                        //wait 5 sec then run this
                        Thread.Sleep(5000);
                        Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                        Environment.Exit(0);
                    });
                    t.Start();
                }
                return new IMessageBase[]{new PlainMessage(sb.ToString())};
            }
            return await base.GetReply(chat);
        }
    }
}
