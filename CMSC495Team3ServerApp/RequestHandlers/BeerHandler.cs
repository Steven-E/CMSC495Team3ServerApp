using System.Net;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Provider;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public class BeerHandler : RequestHandlerBase, IRequestHandler
    {
        public BeerHandler(ILogger log, IConfigProvider config) : base(log, config)
        {
        }

        public override string UrlSegment => "beer";

        public override void Handle(HttpListenerContext httpListenerContext)
        {
            //Find
            //Update
            //Insert
            
        }
    }
}