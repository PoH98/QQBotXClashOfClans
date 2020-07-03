using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Bot.ChatCheck.PlayerAPI
{
    public class 下载:ChatCheckChain
    {
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.StartsWith("/下载 "))
            {
                StringBuilder sb = new StringBuilder();
                string version = chat.Message.Remove(0, 4);
                WebClient wc = new WebClient();
                var html = wc.DownloadString("http://leiren520.com/download/index.html");
                var cocdiv = html.Substring(html.IndexOf("COC下载"));
                cocdiv = cocdiv.Substring(0, cocdiv.IndexOf("多开器下载"));
                bool found = false;
                sb = new StringBuilder();
                foreach (LinkItem i in LinkFinder.Find(cocdiv))
                {
                    sb.AppendLine(i.Text);
                    if (i.Text.Contains(version) || version.Contains(i.Text))
                    {
                        found = true;
                        return "你要的部落冲突下载链接：" + i.Href;
                    }
                }
                if (!found)
                {
                    return Common.CqApi.CqCode_At(chat.FromQQ) + "哈？你确定你要的是部落冲突？我这里只有:\n" + sb.ToString();
                }
                else
                {
                    sb.Clear();
                }
            }
            return base.GetReply(chat);
        }
    }
}
