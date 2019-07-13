using System;
using System.Net;
using System.Net.Http;
using CMSC495Team3ServerApp.ApiClients;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Provider;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public class UpdateHandler : SupportedRequestHandlerBase
    {
        private readonly IUntappdApiClient client;

        public UpdateHandler(ILogger log, IConfigProvider config,
            IErrorResponseFactory errorResponseFactory,
            IUntappdApiClient client) : base(log,
            config, errorResponseFactory)
        {
            this.client = client;

            SupportedActions.Add(HttpMethod.Get, GetAction);
        }

        public override string UrlSegment => "/v-1/";

        private void GetAction(HttpListenerContext context, string[] route)
        {
            try
            {
                if (!route[0].Equals("test"))
                {
                    ErrorResponse
                        .Get(HttpStatusCode.BadRequest)
                        .Handle(context,
                            $"Bad Request - No '/beer/{route[0]}/...' exists");
                    return;
                }

                switch (route[1])
                {
                    case "update":
                        break;
                    //TODO: Test Case for development
                    case "getMe":
                        var request = new RequestWrapper();
                        request.HttpMethod = HttpMethod.Get;
                        request.RelativePath = "user/info/steven-etienne";

                        SendResponse(context, client.Get(request));
                        break;
                    default:
                        ErrorResponse
                            .Get(HttpStatusCode.BadRequest)
                            .Handle(context,
                                $"Bad Request - No '/test failure/{route[0]}/...' exists");
                        break;
                }
            }
            catch (Exception e)
            {
                ErrorResponse.Get(HttpStatusCode.InternalServerError)
                             .Handle(context, e.Message);
            }
        }
    }
}