using System.IO;
using System.Net;
using System.Text;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Provider;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public abstract class RequestHandlerBase : IRequestHandler
    {
        protected readonly IConfigProvider Config;

        protected readonly string ConnectionString;

        protected readonly ILogger Log;

        //protected readonly ISessionManager SessionManager;

        public abstract string UrlSegment { get; }

        protected RequestHandlerBase(ILogger log, IConfigProvider config)//, ISessionManager sessionManager)
        {
            Config = config;

            Log = log;

            //SessionManager = sessionManager;

            ConnectionString = config.DatabaseConnectionString;
        }

        public abstract void Handle(HttpListenerContext httpListenerContext);

        protected void SendResponse(HttpListenerContext httpListenerContext, string responseJson)
        {
            var responseBinary = Encoding.UTF8.GetBytes(responseJson);

            httpListenerContext.Response.ContentEncoding = Encoding.UTF8;
            httpListenerContext.Response.ContentType = "application/json";
            httpListenerContext.Response.StatusCode = (int)HttpStatusCode.OK;
            httpListenerContext.Response.ContentLength64 = responseBinary.Length;
            httpListenerContext.Response.OutputStream.Write(responseBinary, 0, responseBinary.Length);
            httpListenerContext.Response.OutputStream.Close();
        }

        protected string ReadJsonContent(HttpListenerContext httpListenerContext)
        {
            string requestText;

            using (var reader = new StreamReader(httpListenerContext.Request.InputStream,
                httpListenerContext.Request.ContentEncoding))
            {
                requestText = reader.ReadToEnd();
            }

            return requestText;
        }

        protected bool IsAuthorized(HttpListenerContext httpListenerContext, out string bearerToken)
        {
            bearerToken = null;

            var authenticationHeader = httpListenerContext.Request.Headers["Authorization"];

            if (authenticationHeader == null)
            {
                return false;
            }

            if (!authenticationHeader.StartsWith("Bearer"))
            {
                return false;
            }

            bearerToken = authenticationHeader.Substring("Bearer ".Length).Trim();

            return SessionManager.IsValidSession(bearerToken);
        }
    }
}