using System;
using System.Collections.Generic;
using System.Drawing;
using Console = Colorful.Console;

namespace QQBotXClashOfClans_v2
{
    public class Logger
    {
        private static Logger instance;

        public static Logger Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new Logger();
                }
                return instance;
            }
        }

        public void AddLog(LogType type, string Message)
        {
            lock (BaseData.Instance.LogLocker)
            {
                Console.Write("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "] ", Color.Yellow);
                switch (type)
                {
                    case LogType.Debug:
                        Console.Write("[DEBUG]   ", Color.DeepPink);
                        break;
                    case LogType.Info:
                        Console.Write("[INFO]    ", Color.Blue);
                        break;
                    case LogType.Warning:
                        Console.Write("[WARNING] ", Color.Yellow);
                        break;
                    case LogType.Error:
                        Console.Write("[ERROR]   ", Color.Red);
                        break;
                }
                Console.WriteLine(Message.Replace("\n", "\\n"));
            }

        }
    }

    public enum LogType
    {
        Debug,
        Info,
        Warning,
        Error
    }
}
