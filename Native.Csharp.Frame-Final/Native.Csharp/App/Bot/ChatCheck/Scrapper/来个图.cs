using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Native.Csharp.App.Bot.ChatCheck.Scrapper
{
    public class 来个图:ChatCheckChain
    {
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if(chat.Message == "/来图")
            {
                if (File.Exists("Scrapper\\COCBaseScrapper.exe"))
                {
                    //Function active
                    if (!Directory.Exists("Scrapper\\Images\\" + DateTime.Now.ToString("yyyyMMdd")))
                    {
                        if (BaseData.Instance.SendedImage.ContainsKey(chat.FromGroup))
                        {
                            BaseData.Instance.SendedImage[chat.FromGroup].Clear();
                        }
                        ProcessStartInfo scrapper = new ProcessStartInfo()
                        {
                            FileName = Path.Combine(Environment.CurrentDirectory,"Scrapper\\COCBaseScrapper.exe"),
                            Arguments = "-pivix",
                            WorkingDirectory = Path.Combine(Environment.CurrentDirectory, "Scrapper")
                        };
                        Common.CqApi.SendGroupMessage(chat.FromGroup, "正在更新图片列表！");
                        //No files scrapped
                        var p = Process.Start(scrapper);
                        p.WaitForExit();
                    }
                    //Get random image
                    var imagelist = Directory.GetFiles("Scrapper\\Images\\" + DateTime.Now.ToString("yyyyMMdd"));
                    Random rnd = new Random();
                    var selectedImage = imagelist[rnd.Next(0, imagelist.Length)];
                    if (!BaseData.Instance.SendedImage.ContainsKey(chat.FromGroup))
                    {
                        BaseData.Instance.SendedImage.Add(chat.FromGroup, new List<string>());
                        BaseData.Instance.SendedImage[chat.FromGroup].Add(selectedImage);
                    }
                    else
                    {
                        if (BaseData.Instance.SendedImage[chat.FromGroup].Count == imagelist.Length)
                        {
                            BaseData.Instance.SendedImage[chat.FromGroup].Clear();
                        }
                        while (BaseData.Instance.SendedImage[chat.FromGroup].Contains(selectedImage))
                        {
                            //We sended this image, recalculate
                            selectedImage = imagelist[rnd.Next(0, imagelist.Length)];
                        }
                    }
                    Common.CqApi.SendGroupMessage(chat.FromGroup, Common.CqApi.CqCode_Image(selectedImage));
                    return null;
                }
            }
            return base.GetReply(chat);
        }
    }
}
