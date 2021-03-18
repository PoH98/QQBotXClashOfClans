using Mirai_CSharp.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QQBotXClashOfClans_v2.ChatCheck.PlayerAPI
{
    public class 鱼情:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message == "/鱼情")
            {
                WebClient wc = new WebClient();
                JObject result = JObject.Parse(wc.DownloadString("http://clashofclansforecaster.com/STATS.json"));
                StringBuilder sb = new StringBuilder();
                MatchCollection m1 = Regex.Matches(result.Value<JObject>("forecastMessages").Value<string>("chinese-simp"), @"<b>(.*?)</b>", RegexOptions.Singleline);
                MatchCollection m2 = Regex.Matches(result.Value<JObject>("forecastMessages").Value<string>("chinese-simp"), @"(\d+? 小时)", RegexOptions.Singleline);
                sb.Append("当前鱼情报告: " + m1[0].Groups[1].Value + "并且会持续到" + m2[0].Groups[1].Value + "后， 估计接下来鱼情会变成" + m1[1].Groups[1].Value + ", 而" + m2[1].Groups[1].Value + "后情况将会变成" + m1[2].Groups[1].Value);
                return  new IMessageBase[]{new PlainMessage(sb.ToString() + "。\n推荐打鱼程度: " + result.Value<string>("lootIndexString") + "（1-10分）")};
            }
            return await base.GetReply(chat);
        }
    }
}
