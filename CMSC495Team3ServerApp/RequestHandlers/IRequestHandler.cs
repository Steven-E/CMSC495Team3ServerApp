using System.Net;
using System.Text;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Provider;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public interface IRequestHandler
    {
        string UrlSegment { get; }

        void Handle(HttpListenerContext httpListenerContext);
    }

    public abstract class ErrorResponseHandlerBase : IErrorResponseHandler
    {
        protected readonly IConfigProvider Config;
        protected readonly ILogger Log;

        protected ErrorResponseHandlerBase(ILogger log, IConfigProvider config)
        {
            Log = log;
            Config = config;
        }

        protected abstract string Description { get; }
        public abstract HttpStatusCode StatusCode { get; }

        public void Handle(HttpListenerContext httpListenerContext)
        {
            Handle(httpListenerContext, Description);
        }

        public void Handle(HttpListenerContext httpListenerContext, string details)
        {
            var response = httpListenerContext.Response;
            response.StatusCode = (int) StatusCode;
            //response.StatusDescription = details;

            //var outputStream = httpListenerContext.Response.OutputStream;

            //outputStream.Close();

            var responseBinary = Encoding.UTF8.GetBytes(details);

            response.ContentEncoding = Encoding.UTF8;
            response.ContentType = "application/json";
            response.StatusCode = (int) StatusCode;
            response.StatusDescription = StatusCode.ToString();
            response.ContentLength64 = responseBinary.Length;
            response.OutputStream.Write(responseBinary, 0, responseBinary.Length);
            response.OutputStream.Close();
        }
    }

    public interface IErrorResponseHandler
    {
        HttpStatusCode StatusCode { get; }

        void Handle(HttpListenerContext httpListenerContext);

        void Handle(HttpListenerContext httpListenerContext, string details);
    }
}