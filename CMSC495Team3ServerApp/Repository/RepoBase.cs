using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Provider;

namespace CMSC495Team3ServerApp.Repository
{
    public abstract class RepoBase<T>
    {
        protected readonly IConfigProvider Config;
        protected readonly ILogger Log;

        protected RepoBase(ILogger logger, IConfigProvider configProvider)
        {
            Log = logger;
            Config = configProvider;
        }

        public abstract TransactionResult<T> Update(T appObj);
        public abstract TransactionResult<T> Update(T appObj, int referenceKey);
    }

    public class TransactionResult<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Details { get; set; }
    }
}