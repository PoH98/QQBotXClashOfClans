using Native.Csharp.App.Bot;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            清人 q = new 清人();
            Console.WriteLine(q.GetReply(new CqGroupMessageEventArgs(0, "aha", 0, 1078953733, 1993452354, null, "/清人")));
        }
    }
}
