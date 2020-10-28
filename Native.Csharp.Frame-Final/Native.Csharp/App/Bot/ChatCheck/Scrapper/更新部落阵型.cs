using Native.Csharp.Sdk.Cqp.EventArgs;
using System.Diagnostics;
using System.IO;

namespace Native.Csharp.App.Bot.ChatCheck.Scrapper
{
    public class 更新部落阵型 : ChatCheckChain
    {
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message == "/更新部落阵型")
            {
                if (File.Exists("Scrapper\\COCBaseScrapper.exe"))
                {
                    if(Common.CqApi.GetMemberInfo(chat.FromGroup, chat.FromQQ).PermitType != Sdk.Cqp.Enum.PermitType.None)
                    {
                        //Function active
                        var p = Process.Start("Scrapper\\COCBaseScrapper.exe");
                        p.WaitForExit();
                        return "更新完毕！请到官网查看更新后的阵型列表！（可能有延时大概1分钟）";
                    }
                    else
                    {
                        return "你TM没权限更新！";
                    }
                }
            }
            return base.GetReply(chat);
        }
    }
}
