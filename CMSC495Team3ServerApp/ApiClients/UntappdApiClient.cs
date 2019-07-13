using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Provider;
using CMSC495Team3ServerApp.Repository;

namespace CMSC495Team3ServerApp.ApiClients
{
    public class UntappdApiClient : ApiClientBase, IUntappdApiClient
    {
        private readonly string untappdApiBaseUrl;

        private readonly string untappdClientId;
        private readonly string untappdClientSecret;

        public UntappdApiClient(ILogger log, IConfigProvider config, CancellationTokenSource cancellationTokenSource) :
            base(log, config, cancellationTokenSource)
        {
            untappdApiBaseUrl = Config.UntappdApiUrlBase;
            untappdClientId = Config.UntappdAppClientId;
            untappdClientSecret = Config.UntappdAppClientSecret;
            httpClient = new HttpClient();
        }

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
        TransactionResult<ResponseWrapper> Get(RequestWrapper request);
    }
}