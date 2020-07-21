using Native.Csharp.Sdk.Cqp.EventArgs;
using System.Diagnostics;
using System.IO;

namespace Native.Csharp.App.Bot.ChatCheck.CalculatorAPI
{
    public class 计算:ChatCheckChain
    {
        public override string GetReply(CqGroupMessageEventArgs chat)
        {
            if (chat.Message.StartsWith("/计算"))
            {
                var result = CalculateExpression(chat.Message.Trim().Replace("/计算", ""));
                if(result.Contains("Syntax Error"))
                {
                    return "方程式错误";
                }
                else if(result.Contains("variables found"))
                {
                    return result.Replace("variables found", "已记录计算结果");
                }
                else
                {
                    return "计算结果: " + result.Replace("\n", "");
                }
            }
            return base.GetReply(chat);
        }

        private string CalculateExpression(string expression)
        {
            if (!File.Exists("Calculate.exe"))
            {
                File.WriteAllBytes("Calculate.exe", Properties.Resource.Calculate);
            }
            ProcessStartInfo cal = new ProcessStartInfo()
            {
                FileName = "Calculate.exe",
                Arguments = expression,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            Process run = Process.Start(cal);
            return run.StandardOutput.ReadToEnd().Replace("ans = ", "");
        }
	}
}
