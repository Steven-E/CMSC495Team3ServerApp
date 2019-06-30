using System.Collections.Generic;
using System.Linq;
using System.Net;
using CMSC495Team3ServerApp.Logging;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public interface ISupportedRequestHandlerFactory
    {
        IRequestHandler Get(string urlSegment);
    }

    public class SupportedRequestHandlerFactory : ISupportedRequestHandlerFactory
    {
        private readonly ILogger log;

        private readonly IDictionary<string, ISupportedRequestHandler> requestHandlerMap;
        private readonly IErrorResponseFactory errorResponseFactory;

        public SupportedRequestHandlerFactory(ILogger log, IEnumerable<ISupportedRequestHandler> handlers, IErrorResponseFactory errorResponseFactory)
        {
            this.log = log;

            requestHandlerMap = handlers.ToDictionary(handle => handle.UrlSegment, handle => handle);

            this.errorResponseFactory = errorResponseFactory;

        }

        public IRequestHandler Get(string urlSegment)
        {
            if (requestHandlerMap.ContainsKey(urlSegment)) return requestHandlerMap[urlSegment];

            log.Error($"No handler exists for {urlSegment}");

            return errorResponseFactory.Get(HttpStatusCode.NotFound);
        }
    }
}