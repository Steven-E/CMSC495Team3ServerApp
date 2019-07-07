using System.Net;

namespace CMSC495Team3ServerApp.ApiClients
{
    public class ResponseWrapper
    {
        public dynamic Content { get; set; }

        public bool IsSuccess { get; set; }

        public string Reason { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}