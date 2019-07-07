using System.Net;
using System.Net.Http;
using System.Text;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Provider;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public class RobotHandler : SupportedRequestHandlerBase
    {
        private const string ROBOTS_TXT = "User-agent:*\nDisallow: /";

        public RobotHandler(ILogger log, IConfigProvider config, IErrorResponseFactory errorResponseFactory) : base(log,
            config, errorResponseFactory)
        {
            SupportedActions.Add(HttpMethod.Get, GetAction);
        }

        public override string UrlSegment => "/robots.txt";

        private void GetAction(HttpListenerContext httpListenerContext, string[] route)
        {
            if (route.Length > 0)
            {
                ErrorResponse.Get(HttpStatusCode.NotFound).Handle(httpListenerContext);
                return;
            }

            var responseBinary = Encoding.UTF8.GetBytes(ROBOTS_TXT);

            httpListenerContext.Response.ContentEncoding = Encoding.UTF8;
            httpListenerContext.Response.ContentType = "text/plain";
            //httpListenerContext.Response.ContentType = "application/octect-stream";
            httpListenerContext.Response.StatusCode = (int) HttpStatusCode.OK;
            httpListenerContext.Response.ContentLength64 = responseBinary.Length;
            httpListenerContext.Response.AddHeader("content-disposition", "attachment;filename=robots.txt");
            httpListenerContext.Response.OutputStream.Write(responseBinary, 0, responseBinary.Length);
            httpListenerContext.Response.OutputStream.Close();
        }
    }
}