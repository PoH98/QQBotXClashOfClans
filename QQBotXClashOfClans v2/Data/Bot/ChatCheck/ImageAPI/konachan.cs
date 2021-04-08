using Mirai_CSharp.Models;
using Native.Csharp.App.ApiCall;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace QQBotXClashOfClans_v2.Data.Bot.ChatCheck.ImageAPI
{
    public class Konachan:ChatCheckChain
    {
        public override async Task<IEnumerable<IMessageBase>> GetReply(ChainEventArgs chat)
        {
            if (chat.Message == "/图" || chat.Message == "/来图")
            {
                using (var call = new Call(9))
                {
                    var result = call.Request();
                    Logger.Instance.AddLog(LogType.Debug, "已获取图片json" + result.Count + "个");
                    if (result.Count > 0)
                    {
                        var obj = call.GetRandom(result);
                        var path = call.DownloadImage(obj);
                        return new IMessageBase[] { await chat.Session.UploadPictureAsync(UploadTarget.Group, path) };
                    }
                    else
                    {
                        return new IMessageBase[] { new PlainMessage("暂时没有图片可以发哦 ﾍ(;´Д｀ﾍ) 说不定再等等就有了呢！ (ง •̀_•́)ง ") };
                    }
                }
            }
            else if (chat.Message == "/色图")
            {
                using (var call = new Call())
                {
                    var result = call.Request();
                    Logger.Instance.AddLog(LogType.Debug, "已获取图片json" + result.Count + "个");
                    if (result.Count > 0)
                    {
                        var obj = call.GetRandom(result);
                        var path = call.DownloadImage(obj);
                        return new IMessageBase[] { await chat.Session.UploadPictureAsync(UploadTarget.Group, path) };
                    }
                    else
                    {
                        return new IMessageBase[] { new PlainMessage("暂时没有图片可以发哦 ﾍ(;´Д｀ﾍ) 说不定再等等就有了呢！ (ง •̀_•́)ง ") };
                    }
                }
            }
            return await base.GetReply(chat);
        }
    }
}
