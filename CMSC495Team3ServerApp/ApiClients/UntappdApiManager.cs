//using CMSC495Team3ServerApp.RequestHandlers;

//namespace CMSC495Team3ServerApp.ApiClients
//{
//    interface IUntappedApiManager
//    {
        
//    }

//    public class UntappdApiManager : IUntappedApiManager
//    {
//        private readonly UntappdApiCallBackHandler untappdHandler;
//        private readonly IUntappdApiClient client;

//        public UntappdApiManager(ISupportedRequestHandlerFactory supportedHandlerFactory, IUntappdApiClient client)
//        {
//            untappdHandler = supportedHandlerFactory.Get("/api/") as UntappdApiCallBackHandler;
//            this.client = client;

//            untappdHandler.UrlCallBackEvent += UntappdHandlerUrlCallBackEvent;
//        }

//        private void UntappdHandlerUrlCallBackEvent(UntappdClientCallBackEventArgs e)
//        {
//            client.CompleteAuthenticationCycle(e.Code);
//        }
//    }
//}