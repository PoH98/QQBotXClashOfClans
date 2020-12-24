using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Native.Csharp.App.Bot.ChatCheck
{
    public class BaseLink:ChatCheckChain
    {
        public override IEnumerable<string> GetReply(CqGroupMessageEventArgs chat)
        {
            switch (chat.Message)
            {
                case "/八本阵型":
                    return new string[] { "使用浏览器打开此链接: " + GetLink(8) };
                case "/九本阵型":
                    return new string[] { "使用浏览器打开此链接: " + GetLink(9) };
                case "/十本阵型":
                    return new string[] { "使用浏览器打开此链接: " + GetLink(10) };
                case "/十一本阵型":
                    return new string[] { "使用浏览器打开此链接: " + GetLink(11) };
                case "/十二本阵型":
                    return new string[] { "使用浏览器打开此链接: " + GetLink(12) };
                case "/十三本阵型":
                    return new string[] { "使用浏览器打开此链接: " + GetLink(13) };
            }
            return base.GetReply(chat);
        }
        private string GetLink(int 大本等级)
        {
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Debug, "部落冲突阵型DEBUG", "正在获取" + 大本等级 + "本的阵型");
            string 网页 = string.Empty;
            using (WebClient 卧槽 = new WebClient())
            {
                卧槽.Encoding = Encoding.UTF8;
                卧槽.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36");
                try
                {
                    switch (大本等级)
                    {
                        case 8:
                            网页 = 卧槽.DownloadString("https://shimo.im/docs/Ct3c6VYyGvWxJhcg/read");
                            break;
                        case 9:
                            网页 = 卧槽.DownloadString("https://shimo.im/docs/qGKhqyck3Th68ygy/read");
                            break;
                        case 10:
                            网页 = 卧槽.DownloadString("https://shimo.im/docs/PJd8D3JghjXhxCG8//read");
                            break;
                        case 11:
                            网页 = 卧槽.DownloadString("https://shimo.im/docs/WwgChTtHqXCyJGcP/read");
                            break;
                        case 12:
                            网页 = 卧槽.DownloadString("https://shimo.im/docs/vQy9kJQQhr9Vq8v8/read");
                            break;
                        case 13:
                            网页 = 卧槽.DownloadString("https://shimo.im/docs/Gj8wxTcQRwhwhJ69/read");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Error, "链接获取出现错误", ex.ToString());
                }
            }
            var 链接 = LinkFinder.Find(网页);
            List<string> TM的部落链接 = new List<string>();
            foreach (var 阵型链接 in 链接)
            {
                if (阵型链接.Href.Contains("https://link.clashofclans.com/"))
                {
                    TM的部落链接.Add(阵型链接.Href);
                }
            }
            Common.CqApi.AddLoger(Sdk.Cqp.Enum.LogerLevel.Info, "部落冲突阵型获取", "已获取" + TM的部落链接.Count + "阵型");
            Random rnd = new Random();
            int rndResult = rnd.Next(0, TM的部落链接.Count);
            return "https://link.clashofclans.com/cn?" + TM的部落链接[rndResult].Remove(0, "https://link.clashofclans.com/cn?".Length);
        }
    }
}
