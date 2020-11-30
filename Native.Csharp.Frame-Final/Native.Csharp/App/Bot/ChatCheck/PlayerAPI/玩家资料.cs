using CocNET;
using CocNET.Interfaces;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Native.Csharp.App.Bot.ChatCheck.PlayerAPI
{
    class 玩家资料:ChatCheckChain
    {
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.ToLower().Contains("/玩家资料 #"))
            {
                try
                {
                    var cocid = chat.Message.Split(' ').Where(x => x.Contains("#")).Last().Replace(" ", "");
                    if (!BaseData.CheckIP())
                    {
                        TokenApi.GetNewToken();
                    }
                    if (cocid == BaseData.Instance.config["部落冲突"][chat.FromGroup.ToString()])
                    {
                        return Common.CqApi.CqCode_At(chat.FromQQ) + "你当我傻？拿部落标签给我查玩家？草你马的";
                    }
                    ICocCorePlayers players = BaseData.Instance.container.Resolve<ICocCorePlayers>();
                    var player = players.GetPlayer(cocid);
                    if (player != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("大本营等级：" + player.TownHallLevel + "，名字：" + player.Name);
                        sb.AppendLine("进攻次数: " + player.AttackWins + "，防御次数: " + player.DefenseWins + "，战星: " + player.WarStars);
                        sb.AppendLine("==================================");
                        sb.AppendLine("兵力：");
                        foreach (var troop in player.Troops)
                        {
                            try
                            {
                                sb.AppendLine("* " + BaseData.Instance.config["兵种翻译"][troop.Name.Replace(" ", "_")] + " - " + troop.Level + "级");
                            }
                            catch
                            {
                                sb.AppendLine("* " + troop.Name + " - " + troop.Level + "级");
                            }
                        }
                        sb.AppendLine("==================================");
                        sb.AppendLine("药水：");
                        foreach (var spell in player.Spells)
                        {
                            try
                            {
                                sb.AppendLine("* " + BaseData.Instance.config["兵种翻译"][spell.Name.Replace(" ", "_")] + " - " + spell.Level + "级");
                            }
                            catch
                            {
                                sb.AppendLine("* " + spell.Name + " - " + spell.Level + "级");
                            }
                        }
                        sb.AppendLine("==================================");
                        sb.AppendLine("英雄：");
                        foreach (var hero in player.Heroes)
                        {
                            try
                            {
                                sb.AppendLine("* " + BaseData.Instance.config["兵种翻译"][hero.Name.Replace(" ", "_")] + " - " + hero.Level + "级");
                            }
                            catch
                            {
                                sb.AppendLine("* " + hero.Name + " - " + hero.Level + "级");
                            }
                        }
                        string rndName = Path.Combine("Buffer\\" + Path.GetRandomFileName());
                        Convert_Text_to_Image(sb.ToString(), "Times New Roman", 13).Save(rndName);
                        return Common.CqApi.CqCode_At(chat.FromQQ) + "你要的资料 [bmp:" + rndName + "] ";
                    }
                    else
                    {
                        return "未知的部落冲突ID，无法搜索该玩家资料！";
                    }
                }
                catch (Exception ex)
                {
                    return "请确保发送/PlayerAPI时是/PlayerAPI 玩家标签！错误资料：" + ex.ToString();
                }
            }
            return base.GetReply(chat);
        }

        private static Bitmap Convert_Text_to_Image(string txt, string fontname, int fontsize)
        {
            //creating bitmap image
            Bitmap bmp = new Bitmap(1, 1);

            //FromImage method creates a new Graphics from the specified Image.
            Graphics graphics = Graphics.FromImage(bmp);
            // Create the Font object for the image text drawing.
            Font font = new Font(fontname, fontsize);
            // Instantiating object of Bitmap image again with the correct size for the text and font.
            SizeF stringSize = graphics.MeasureString(txt, font);
            bmp = new Bitmap(bmp, (int)stringSize.Width, (int)stringSize.Height);
            graphics = Graphics.FromImage(bmp);

            /* It can also be a way
           bmp = new Bitmap(bmp, new Size((int)graphics.MeasureString(txt, font).Width, (int)graphics.MeasureString(txt, font).Height));*/

            //Draw Specified text with specified format 
            graphics.FillRectangle(new SolidBrush(Color.Black), new RectangleF(0, 0, (float)stringSize.Width, (float)stringSize.Height));
            graphics.DrawString(txt, font, Brushes.White, 0, 0);
            font.Dispose();
            graphics.Flush();
            graphics.Dispose();
            return bmp;     //return Bitmap Image 
        }
    }
}
