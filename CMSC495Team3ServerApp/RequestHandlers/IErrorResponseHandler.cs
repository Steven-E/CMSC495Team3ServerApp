using System.Net;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public interface IErrorResponseHandler : IRequestHandler
    {
        HttpStatusCode StatusCode { get; }

        //void Handle(HttpListenerContext httpListenerContext);

        void Handle(HttpListenerContext httpListenerContext, string details);
    }
}