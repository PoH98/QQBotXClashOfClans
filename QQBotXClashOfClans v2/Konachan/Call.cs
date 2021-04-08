using Native.Csharp.App.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace Native.Csharp.App.ApiCall
{
    public class Call:IDisposable
    {
        private WebClient wc;
        private string channel;
        private int score, page = 1;
        private XmlSerializer serializer = new XmlSerializer(typeof(Posts));
        public Call(int score = 0)
        {
            wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36");
            this.score = score;
        }

        public void Dispose()
        {
            wc.Dispose();
        }

        public List<Post> Request()
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
            var result = serializer.Deserialize(wc.OpenRead(url)) as Posts;
            var returnobj = result.Post.Where(x => x.rating.ToLower() == "s").ToList();
            //Remove sended object
            foreach (var obj in result.Post)
            {
                if (Singleton.GetInstance().list[channel].Contains(obj.id))
                {
                    var remove = returnobj.Where(x => x.id == obj.id).FirstOrDefault();
                    if (remove != null)
                    {
                        returnobj.Remove(remove);
                    }
                }
            }
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

        public Post GetRandom(List<Post> list)
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

        public string DownloadImage(Post json)
        {
            if (!Directory.Exists("Buffer"))
            {
                Directory.CreateDirectory("Buffer");
            }
            foreach(var file in Directory.GetFiles("Buffer"))
            {
                FileInfo info = new FileInfo(file);
                if((DateTime.Now - info.CreationTime).Days > 1)
                {
                    info.Delete();
                }
            }
            string randomName = "Buffer\\"+ Path.GetRandomFileName();
            wc.DownloadFile(json.jpeg_url, randomName);
            return randomName;
        }
    }
}
