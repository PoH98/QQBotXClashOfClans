using CocNET.Interfaces;
using Native.Csharp.App.Bot;
using Unity;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            BaseData.LoadCOCData();
            var data = BaseData.Instance.container.Resolve<ICocCorePlayers>();
            data.GetPlayer("");
        }
    }
}
