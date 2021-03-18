using System;
using System.Collections.Generic;

namespace QQBotXClashOfClans_v2
{
    public class BaseLinkData
    {
        public BaseLinkData(DateTime LastUpdate, List<string> Links)
        {
            this.LastUpdate = LastUpdate;
            Logger.Instance.AddLog(LogType.Debug, "已获取" + Links.Count + "阵型");
            this.Links = Links;
        }
        public DateTime LastUpdate { get; set; }

        public List<string> Links { get; }

        ~BaseLinkData()
        {
            try
            {
                Links.Clear();
            }
            catch
            {

            }
        }

        public string GetLink()
        {
            Random rnd = new Random();
            var index = rnd.Next(0, Links.Count);
            var result = Links[index].Clone().ToString();
            Links.RemoveAt(index);
            return result;
        }
    }
}
