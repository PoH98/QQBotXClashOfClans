using Native.Csharp.App.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Native.Csharp.App.ApiCall
{
    public class Call:IDisposable
    {
        private WebClient wc;
        private string channel;
        private int page = 1;
        private XmlSerializer serializer = new XmlSerializer(typeof(Posts));
        public Call()
        {
            wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36");
        }

        public void Dispose()
        {
            wc.Dispose();
        }

        public async Task<IList<Post>> Request(int score = 0)
        {
            Random rnd = new Random();
            string url = "";
            if (rnd.Next(score, 10) < 6)
            {
                channel = "konachan";
                url = "https://konachan.net/post.xml?limit=50";
            }
            else
            {
                channel = "yandere";
                url = "https://yande.re/post.xml?limit=50";
            }
            if (!Singleton.GetInstance().list.ContainsKey(channel))
            {
                Singleton.GetInstance().list.Add(channel, new List<uint>());
            }
            Load:
            var result = serializer.Deserialize(await wc.OpenReadTaskAsync(url)) as Posts;
            IList<Post> returnobj;
            if(score < 6)
            {
                returnobj = result.Post.Where(x => x.rating.ToLower() == "s" || x.rating == "q").ToList();
            }
            else
            {
                returnobj = result.Post.Where(x => x.rating.ToLower() == "s").ToList();
            }
            //Remove sended object
            var instance = Singleton.GetInstance();
           Parallel.ForEach(result.Post, (post) =>
           {
               if (instance.list[channel].Contains(post.id))
               {
                   var remove = returnobj.FirstOrDefault(x => x.id == post.id);
                   if (remove != null)
                   {
                       returnobj.Remove(remove);
                   }
               }
           });
            if(returnobj.Count < 1)
            {
                page++;
                switch (channel)
                {
                    case "konachan":
                        url = "https://konachan.net/post.xml?limit=50&page=" + page;
                        break;
                    case "yandere":
                        url = "https://yande.re/post.xml?limit=50&page=" + page;
                        break;
                }
                goto Load;
            }
            return returnobj;
        }

        public Post GetRandom(IList<Post> list)
        {
            var rnd = new Random();
            var obj = list[rnd.Next(0, list.Count)];
            Singleton.GetInstance().list[channel].Add(obj.id);
            if (Singleton.GetInstance().list.Count > 100)
            {
                Singleton.GetInstance().list.Clear();
            }
            return obj as Post;
        }

        public async Task<Stream> DownloadImage(Post json)
        {
            return await wc.OpenReadTaskAsync(json.jpeg_url);
        }
    }
}
