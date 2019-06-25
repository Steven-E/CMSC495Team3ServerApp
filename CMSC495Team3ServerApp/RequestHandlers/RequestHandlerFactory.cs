using System.Collections.Generic;
using System.Linq;
using CMSC495Team3ServerApp.Logging;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public class RequestHandlerFactory : IRequestHandlerFactory
    {
        private readonly ILogger log;

        private readonly IDictionary<string, IRequestHandler> requestHandlerMap;

        public RequestHandlerFactory(ILogger log, IEnumerable<IRequestHandler> handlers)
        {
            this.log = log;

            requestHandlerMap = handlers.ToDictionary(handle => handle.UrlSegment, handle => handle);
        }

        public IRequestHandler Get(string urlSegment)
        {
            if (requestHandlerMap.ContainsKey(urlSegment))
            {
                return requestHandlerMap[urlSegment];
            }

            log.Error($"No handler exists for {urlSegment}");

            return requestHandlerMap["NOT_FOUND"];
        }
    }
}