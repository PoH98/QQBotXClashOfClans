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
                return  new IMessageBase[]{new PlainMessage(result.Value<JObject>("forecastMessages").Value<string>("chinese-simp").Replace("<b>", "").Replace("</b>","").Replace(" ","") + "。\n推荐打鱼程度: " + result.Value<string>("lootIndexString") + "（1-10分）")};
            }
            return await base.GetReply(chat);
        }
    }
}
