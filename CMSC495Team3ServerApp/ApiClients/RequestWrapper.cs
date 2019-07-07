using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CMSC495Team3ServerApp.ApiClients
{
    public class RequestWrapper
    {
        public AuthenticationHeaderValue AuthenticationHeaderValue { get; set; }

        public HttpContent HttpContent { get; set; }

        public HttpMethod HttpMethod { get; set; }

        public List<KeyValuePair<string, string>> QueryParameters { get; set; }

        public string RelativePath { get; set; }

        public Dictionary<string, string> RequestSpecificHeaders { get; set; }

        public RequestWrapper()
        {
            RequestSpecificHeaders = new Dictionary<string, string>();
            QueryParameters = new List<KeyValuePair<string, string>>();
        }
    }
}