using System.Net;
using System.Text;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Provider;

namespace CMSC495Team3ServerApp.RequestHandlers
{
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

    //405 Method Not Allowed
    public class MethodNotAllowedHandler : ErrorResponseHandlerBase
    {
        public MethodNotAllowedHandler(ILogger log, IConfigProvider config) : base(log, config)
        {
        }

        protected override string Description => "Method not allowed";
        public override HttpStatusCode StatusCode => HttpStatusCode.MethodNotAllowed;
    }

    //500 Internal Server Error
    public class ServerErrorHandler : ErrorResponseHandlerBase
    {
        public ServerErrorHandler(ILogger log, IConfigProvider config) : base(log, config)
        {
        }

        protected override string Description => "Internal Server Error";
        public override HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
    }

    //400 Bad Request
    public class BadRequestHandler : ErrorResponseHandlerBase
    {
        public BadRequestHandler(ILogger log, IConfigProvider config) : base(log, config)
        {
        }

        protected override string Description => "Bad Request";
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    }

    //404 Not Found
    public class NotFoundHandler : ErrorResponseHandlerBase
    {
        public NotFoundHandler(ILogger log, IConfigProvider config) : base(log, config)
        {
        }

        protected override string Description => "Requested resource does not exist or cannot be found.";
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    }
}