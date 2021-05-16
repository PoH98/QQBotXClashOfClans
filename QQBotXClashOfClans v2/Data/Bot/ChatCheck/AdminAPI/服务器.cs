using Mirai_CSharp.Models;
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
                sb.Append(BaseData.Instance.cpuUsage.ToString("N0") + "%\n");
                sb.Append("服务器内存使用: ");
                sb.Append(BaseData.Instance.ramUsage.ToString("N0") + "%\n");
                sb.Append("服务器CPU名字: ");
                sb.Append(BaseData.Instance.cpuName);
                if (BaseData.Instance.checkClanWar == null)
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
