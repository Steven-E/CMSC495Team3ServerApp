using CMSC495Team3ServerApp.ApiClients;
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

            Bind<ISupportedRequestHandlerFactory>().To<SupportedRequestHandlerFactory>().InSingletonScope();

            Bind<IErrorResponseFactory>().To<ErrorResponseFactory>().InSingletonScope();

            // Repo bindings
            Bind<IBeerRepo>().To<BeerRepo>();
            Bind<IBreweryRepo>().To<BreweryRepo>();
            Bind<IUserInfoRepo>().To<UserInfoRepo>();
            Bind<IUserBeerRankingRepo>().To<UserBeerRankingRepo>();
            Bind<ISocialMediaRepo>().To<SocialMediaRepo>();
            Bind<ISocialMediaAccountRepo>().To<SocialMediaAccountRepo>();

            // Api Client binding
            Bind<IUntappdApiClient>().To<UntappdApiClient>().InSingletonScope();


            Kernel.Bind(x =>
            {
                x.FromThisAssembly()
                 .SelectAllClasses()
                 .InheritedFrom<IErrorResponseHandler>()
                 .BindAllInterfaces()
                 .Configure(b => { b.InSingletonScope(); });
            });

            Kernel.Bind(
                x =>
                {
                    x.FromThisAssembly()
                     .SelectAllClasses()
                     .InheritedFrom<ISupportedRequestHandler>()
                     .BindAllInterfaces()
                     .Configure(b => { b.InSingletonScope(); });
                });
        }
    }
}