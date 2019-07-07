//using System;
//using System.Net;
//using System.Net.Http;
//using CMSC495Team3ServerApp.Logging;
//using CMSC495Team3ServerApp.Models;
//using CMSC495Team3ServerApp.Provider;
//using Newtonsoft.Json;

//namespace CMSC495Team3ServerApp.RequestHandlers
//{
//    public class UntappdClientCallBackEventArgs : EventArgs
//    {
//        public string Code { get; set; }
//    }

//    public class UntappdApiCallBackHandler : SupportedRequestHandlerBase
//    {
//        public delegate void UntappdClientCallBackEvent(UntappdClientCallBackEventArgs e);

//        public UntappdApiCallBackHandler(ILogger log, IConfigProvider config,
//            IErrorResponseFactory errorResponseFactory) : base(log, config, errorResponseFactory)
//        {
//            SupportedActions.Add(HttpMethod.Get, GetAction);

//            SetupDocumentation();
//        }

//        public override string UrlSegment => "/api/";

//        public event UntappdClientCallBackEvent UrlCallBackEvent;

//        //public override string UrlSegment => Config.ExposedHttpUrl + Config.UntappdClientCallBackRoute;

//        private void SetupDocumentation()
//        {
//            //http://REDIRECT_URL?code=CODE
//            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "?code={untappdClientCode}", "URL", "NULL",
//                null));
//        }

//        private void OnUrlCallBackEvent(string code)
//        {
//            UrlCallBackEvent?.Invoke(new UntappdClientCallBackEventArgs
//            {
//                Code = code
//            });
//        }


//        private void GetAction(HttpListenerContext context, string[] route)
//        {
//            try
//            {
//                if (route.Length > 1 && string.Equals(Config.UntappdClientCallBackRoute, string.Join("/", route)))
//                {
//                    //Handler stuff
//                    var queryString = context.Request.QueryString;

//                    var code = queryString["code"];

//                    if (string.IsNullOrWhiteSpace(code)) SendBadRequest(context);

//                    SendOKResponseAndPayload(context, null);

//                    OnUrlCallBackEvent(code);
//                }
//                else if (route.Length == 0 && string.Equals("help", route[0]))
//                {
//                    SendOKResponseAndPayload(context, JsonConvert.SerializeObject(EndpointDocumentation));
//                }
//                else
//                {
//                    SendBadRequest(context);
//                }
//            }
//            catch (Exception e)
//            {
//                var request = context.Request;
//                var rawUrl = request.RawUrl;
//                var client = request.RemoteEndPoint;
//                var content = ReadJsonContent(context);

//                Log.Error($"Encountered exception while trying to process '{UrlSegment}' request. Request Details - Received '{request.HttpMethod}' for URL '{rawUrl}' from client - " +
//                         $"{client.Address}:{client.Port}, and content - '{content}'", e);

//                ErrorResponse.Get(HttpStatusCode.BadRequest).Handle(context);
//            }
//        }
//    }
//}