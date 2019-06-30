using System.Collections.Generic;
using System.Linq;
using System.Net;
using CMSC495Team3ServerApp.Logging;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public interface IErrorResponseFactory
    {
        IErrorResponseHandler Get(HttpStatusCode statusCode);
    }

    public class ErrorResponseFactory : IErrorResponseFactory
    {
        private readonly IDictionary<HttpStatusCode, IErrorResponseHandler> errorHandlersMap;
        private readonly ILogger log;

        public ErrorResponseFactory(ILogger log, IEnumerable<IErrorResponseHandler> errorHandlers)
        {
            this.log = log;

            errorHandlersMap = errorHandlers.ToDictionary(handler => handler.StatusCode, handler => handler);
        }

        public IErrorResponseHandler Get(HttpStatusCode statusCode)
        {
            if (errorHandlersMap.ContainsKey(statusCode)) return errorHandlersMap[statusCode];

            log.Error($"No handler exists for Status Code - '{statusCode}'");

            return errorHandlersMap[HttpStatusCode.InternalServerError];
        }
    }
}