using System.Net;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public interface ISupportedRequestHandler : IRequestHandler
    {
        string UrlSegment { get; }

        //void Handle(HttpListenerContext httpListenerContext);
    }

    public interface IRequestHandler
    {
        void Handle(HttpListenerContext httpListenerContext);
    }
}