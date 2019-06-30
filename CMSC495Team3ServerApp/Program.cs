using System.Threading.Tasks;
using CMSC495Team3ServerApp.Module;
using Ninject;

namespace CMSC495Team3ServerApp
{
    internal class Program
    {
        private static readonly StandardKernel kernel = new StandardKernel(new ServerAppModule());

        private static IServerAppWorker service;

        public static int Main(string[] args)
        {
            service = kernel.Get<IServerAppWorker>();

            return RunAsync().Result;
        }

        private static async Task<int> RunAsync()
        {
            await service.Start();
            
            return service.CancellationTokenSource.IsCancellationRequested ? 1067 : 0;
        }
    }
}