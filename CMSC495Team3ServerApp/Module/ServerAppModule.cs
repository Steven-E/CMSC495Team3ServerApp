using CMSC495Team3ServerApp.ApiClients;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Provider;
using CMSC495Team3ServerApp.Repository;
using CMSC495Team3ServerApp.RequestHandlers;
using Ninject;
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

            




            //var temp = Kernel.Get<ISupportedRequestHandlerFactory>().Get(/"api/")
            //var temp = Kernel.Get<IServerAppWorker>().CancellationTokenSource;

            ////var cts = Kernel.Get<IServerAppWorker>().CancellationTokenSource;
            ////this.Kernel.
                
            //    //this.KernelInstance.Get<IServerAppWorker>().CancellationTokenSource;

            ////var api = Kernel.Get<ISupportedRequestHandlerFactory>().Get("api/");
            //var api = this.KernelInstance.Get<ISupportedRequestHandlerFactory>().Get("api/");

            //Bind<IUntappdApiClient>().To<UntappdApiClient>()
            //    .WithConstructorArgument("cancellationTokenSource", cts)
            //    .WithConstructorArgument("restHandler", api);
        }
    }
}