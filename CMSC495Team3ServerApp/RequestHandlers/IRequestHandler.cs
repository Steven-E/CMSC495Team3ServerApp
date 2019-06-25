using System.Net;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public interface IRequestHandler
    {
        string UrlSegment { get; }

        void Handle(HttpListenerContext httpListenerContext);
    }
}