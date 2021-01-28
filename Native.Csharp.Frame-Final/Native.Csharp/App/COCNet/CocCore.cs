using CocNET.Interfaces;
using CocNET.Methods;
using Unity;
using CocNET.Services;
using Native.Csharp.App.Bot;

namespace CocNET
{
    public class CocCore
    {
        public IUnityContainer Container { get; set; }

        public CocCore(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TokenApi.GetNewToken();
                token = BaseData.Instance.config["部落冲突"]["Token"];
            }
            if (Container == null)
            {
                Container = new UnityContainer();
            }
            Container.RegisterInstance<Request>("Request", new Request(token));
            Container.RegisterInstance<ICocCoreClans>(new CocCoreClans(Container.Resolve<Request>("Request")));
            //Container.Register<ICocCoreLocations>(new CocCoreLocations(Container.ResolveNamed<Request>("Request")));
            //Container.Register<ICocCoreLeagues>(new CocCoreLeagues(Container.ResolveNamed<Request>("Request")));
            Container.RegisterInstance<ICocCorePlayers>(new CocCorePlayers(Container.Resolve<Request>("Request")));
        }
    }
}