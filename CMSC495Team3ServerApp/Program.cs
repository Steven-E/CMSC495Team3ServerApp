using System.Threading.Tasks;
using CMSC495Team3ServerApp.ApiClients;
using CMSC495Team3ServerApp.Module;
using CMSC495Team3ServerApp.RequestHandlers;
using Ninject;
using Ninject.Planning.Bindings;

namespace CMSC495Team3ServerApp
{
    internal class Program
    {
        private static readonly StandardKernel kernel = new StandardKernel(new ServerAppModule());

        private static IServerAppWorker service;

        public static int Main(string[] args)
        {
            service = kernel.Get<IServerAppWorker>();

            //var cts = Kernel.Get<IServerAppWorker>().CancellationTokenSource;
            //this.Kernel.

            //this.KernelInstance.Get<IServerAppWorker>().CancellationTokenSource;

            //var api = Kernel.Get<ISupportedRequestHandlerFactory>().Get("api/");
            //var api = this.KernelInstance.Get<ISupportedRequestHandlerFactory>().Get("api/");

            //Bind<IUntappdApiClient>().To<UntappdApiClient>()
            //    .WithConstructorArgument("cancellationTokenSource", cts)
            //    .WithConstructorArgument("restHandler", api);

            //var cts = service.CancellationTokenSource;
            //var api = kernel.Get<ISupportedRequestHandlerFactory>().Get("/api/");

            //kernel.Bind<IUntappdApiClient>().To<UntappdApiClient>().WithConstructorArgument("cancellationTokenSource", cts)
            //    .WithConstructorArgument("restHandler", api);


            return RunAsync().Result;
        }

        private static async Task<int> RunAsync()
        {
            await service.Start();
            
            return service.CancellationTokenSource.IsCancellationRequested ? 1067 : 0;
        }
    }
}