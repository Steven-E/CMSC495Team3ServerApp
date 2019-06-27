using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Provider;
using CMSC495Team3ServerApp.Repository;
using CMSC495Team3ServerApp.RequestHandlers;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace CMSC495Team3ServerApp.Module
{
    public class ServerAppModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILogger>().To<Logger>().InSingletonScope();

            Bind<IConfigProvider>().To<ConfigProvider>().InSingletonScope();

            Bind<IServerAppWorker>().To<ServerAppWorker>().InSingletonScope();

            Bind<IRequestHandlerFactory>().To<RequestHandlerFactory>().InSingletonScope();

            Bind<IUserInfoRepo>().To<UserInfoRepo>();
            Bind<IUserBeerRankingRepo>().To<UserBeerRankingRepo>();
            Bind<ISocialMediaAccountRepo>().To<SocialMediaAccountRepo>();

            Kernel.Bind(
                x =>
                {
                    x.FromThisAssembly()
                        .SelectAllClasses()
                        .InheritedFrom<IRequestHandler>()
                        .BindAllInterfaces();
                });
        }
    }
}