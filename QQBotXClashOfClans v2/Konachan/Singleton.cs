using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.App.Model
{
    public class Singleton
    {
        private static Singleton instance;

        public static Singleton GetInstance()
        {
            if(instance == null)
            {
                instance = new Singleton();
            }
            return instance;
        }

        public Dictionary<string,List<uint>> list = new Dictionary<string, List<uint>>();
    }
}
