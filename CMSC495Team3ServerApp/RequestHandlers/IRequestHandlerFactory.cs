using System.Net;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public interface IRequestHandlerFactory
    {
        IRequestHandler Get(string urlSegment);
    }

    public interface IErrorResponseFactory
    {
        IErrorResponseHandler Get(HttpStatusCode statusCode);
    }
}