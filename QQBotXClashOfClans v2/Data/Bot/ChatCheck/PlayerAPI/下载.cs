
using Mirai_CSharp.Models;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QQBotXClashOfClans_v2.ChatCheck.PlayerAPI
{
    public class 下载:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message.StartsWith("/下载 "))
            {
                string version = chat.Message.Remove(0, 4);
                WebClient wc = new WebClient();
                var html = wc.DownloadString("http://leiren520.com/download/index.html");
                var cocdiv = html[html.IndexOf("COC下载")..];
                cocdiv = cocdiv.Substring(0, cocdiv.IndexOf("多开器下载"));
                bool found = false;
                StringBuilder sb = new StringBuilder();
                foreach (LinkItem i in LinkFinder.Find(cocdiv))
                {
                    sb.AppendLine(i.Text);
                    if (i.Text.Contains(version) || version.Contains(i.Text))
                    {
                        found = true;
                        return new IMessageBase[]{ new PlainMessage("你要的部落冲突下载链接：" + i.Href)};
                    }
                }
                if (!found)
                {
                    return new IMessageBase[]{ new AtMessage(chat.FromQQ), new PlainMessage("哈？你确定你要的是部落冲突？我这里只有:\n" + sb.ToString())};
                }
                else
                {
                    sb.Clear();
                }
            }
            return await base.GetReply(chat);
        }
    }
}
