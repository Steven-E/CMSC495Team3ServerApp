using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Models.Untappd;
using CMSC495Team3ServerApp.Provider;
using CMSC495Team3ServerApp.Repository;
using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.ApiClients
{
    public class UntappdApiClient : ApiClientBase, IUntappdApiClient
    {
        //private readonly string untappdClientCallBackRoute;
        //private readonly string untappdOAuthAppAuthenticationRoute;

        private readonly string untappdClientId;
        private readonly string untappdClientSecret;
        private readonly string untappdApiBaseUrl;

        //private string untappdAccessToken;

        public UntappdApiClient(ILogger log, IConfigProvider config, CancellationTokenSource cancellationTokenSource) :
            base(log, config, cancellationTokenSource)
        {
            untappdApiBaseUrl = Config.UntappdApiUrlBase;
            untappdClientId = Config.UntappdAppClientId;
            untappdClientSecret = Config.UntappdAppClientSecret;
            //untappdClientCallBackRoute = Config.PublicAddressBase + Config.UntappdClientCallBackRoute;
            //untappdOAuthAppAuthenticationRoute = Config.UntappdOAuthAppAuthenticationRoute;
            //untappdClientCallBackRoute = config.UntappdClientCallBackUrl;
            //api/Untappd/CallBack

            httpClient = new HttpClient();
        }

        //public bool IsAuthenticated { get; private set; }
        //public override TransactionResult<ResponseWrapper> AuthenticateToApi()
        //{
        //    return InitiateAuthenticationCycle();
        //}

        ////
        //var request = new RequestWrapper
        //{
        //    HttpMethod = HttpMethod.Get,
        //    RelativePath = untappdOAuthAppAuthenticationRoute
        //};

        //request.QueryParameters = new List<KeyValuePair<string, string>>
        //{
        //    new KeyValuePair<string, string>("client_id", untappdClientId),
        //    new KeyValuePair<string, string>("response_type", "code"),
        //    new KeyValuePair<string, string>("redirect_url", untappdClientCallBackRoute)
        //};

        //var response = base.SendRestRequest(request).Result;

        //if (response.IsSuccess)
        //{
        //    // do stuff

        //    //var object = JsonConvert.DeserializeObject<T>(response.Content.ToString())

        //    //wait
        //}

        //throw new NotImplementedException();

        //public TransactionResult<ResponseWrapper> InitiateAuthenticationCycle()
        //{
        //    //
        //    var request = new RequestWrapper
        //    {
        //        HttpMethod = HttpMethod.Get,
        //        RelativePath = untappdOAuthAppAuthenticationRoute
        //    };

        //    request.QueryParameters = new List<KeyValuePair<string, string>>
        //    {
        //        new KeyValuePair<string, string>("client_id", untappdClientId),
        //        new KeyValuePair<string, string>("response_type", "code"),
        //        new KeyValuePair<string, string>("redirect_url", untappdClientCallBackRoute)
        //    };

        //    var response = base.SendRestRequest(request).Result;

        //    //if (response.IsSuccess)
        //    //{
        //    //    // do stuff

        //    //    //var object = JsonConvert.DeserializeObject<T>(response.Content.ToString())

        //    //    //wait
        //    //}

        //    return new TransactionResult<ResponseWrapper>() {Data = response, Success = response.IsSuccess};
        //}

        //public void CompleteAuthenticationCycle(string code)
        //{
        //    if (IsAuthenticated)
        //        return;

        //    var request = new RequestWrapper
        //    {
        //        HttpMethod = HttpMethod.Get,
        //        RelativePath = untappdOAuthAppAuthenticationRoute
        //    };

        //    request.QueryParameters = new List<KeyValuePair<string, string>>
        //    {
        //        new KeyValuePair<string, string>("client_id", untappdClientId),
        //        new KeyValuePair<string, string>("client_secret", untappdClientSecret),
        //        new KeyValuePair<string, string>("response_type", "code"),
        //        new KeyValuePair<string, string>("redirect_url", untappdClientCallBackRoute),
        //        new KeyValuePair<string, string>("code", code)
        //    };

        //    var response = base.SendRestRequest(request).Result;

        //    if (response.IsSuccess)
        //    {
        //        // do stuff

        //        var dynamicUntappdAuth =
        //            JsonConvert.DeserializeObject<UntappdAuthResponse>(response.Content.ToString());

        //        UntappdAuthResponse untappdAuthResponse = dynamicUntappdAuth;

        //        if (untappdAuthResponse.meta.http_code == (int) HttpStatusCode.OK)
        //            untappdAccessToken = untappdAuthResponse.response.access_token;
        //    }

        //    Log.Info($"Received following response content: {JsonConvert.SerializeObject(response)}");
        //}

        public TransactionResult<ResponseWrapper> Get(RequestWrapper request)
        {
            request.RelativePath = untappdApiBaseUrl + request.RelativePath;

                request.QueryParameters.Add(new KeyValuePair<string, string>("client_id", untappdClientId));
                request.QueryParameters.Add(new KeyValuePair<string, string>("client_secret", untappdClientSecret));

            var response = base.SendRestRequest(request).Result;

            return new TransactionResult<ResponseWrapper>
                {Data = response, Success = response.IsSuccess, Details = response.Reason};
        }
    }

    public interface IUntappdApiClient
    {
        //bool IsAuthenticated { get; }

        //TransactionResult<ResponseWrapper> AuthenticateToApi();

        TransactionResult<ResponseWrapper> Get(RequestWrapper request);



        //void CompleteAuthenticationCycle(string code);
    }
}