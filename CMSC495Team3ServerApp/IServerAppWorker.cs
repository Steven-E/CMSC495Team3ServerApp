using System.Threading;
using System.Threading.Tasks;

namespace CMSC495Team3ServerApp
{
    public interface IServerAppWorker
    {
        CancellationTokenSource CancellationTokenSource { get; }

        Task Start();

        void Stop();
    }
}