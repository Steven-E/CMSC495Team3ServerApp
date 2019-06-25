using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Provider;
using CMSC495Team3ServerApp.RequestHandlers;
using CMSC495Team3ServerApp.UtilityClasses;

namespace CMSC495Team3ServerApp
{
    public class ServerAppWorker : IServerAppWorker
    {
        public CancellationTokenSource CancellationTokenSource { get; }

        private readonly IConfigProvider config;

        private readonly ILogger log;

        private readonly HttpListener httpListener;

        private readonly SemaphoreSlim requestSemaphoreSlim;

        private readonly IRequestHandlerFactory requestHandlerFactory;

        private readonly QueueProcessor<HttpListenerContext> requestQueue;


        public ServerAppWorker(ILogger log, IConfigProvider config, IRequestHandlerFactory requestHandlerFactory)
        {
            CancellationTokenSource = new CancellationTokenSource();

            this.config = config;
            this.log = log;
            this.requestHandlerFactory = requestHandlerFactory;


            httpListener = new HttpListener();

            httpListener.Prefixes.Add(config.ExposedHttpUrl);

            //TODO: Come back and add this to the configprovider
            requestSemaphoreSlim = new SemaphoreSlim(20, 20);

            requestQueue = new QueueProcessor<HttpListenerContext>(RequestProcessor, 
                CancellationTokenSource.Token, 
                int.MaxValue);
        }

        private void RequestProcessor(HttpListenerContext context)
        {
            requestSemaphoreSlim.Wait();

            Task.Run(() => { RequestThreadEntry(context); }, CancellationTokenSource.Token)
                .ContinueWith(x => { requestSemaphoreSlim.Release(); }, CancellationTokenSource.Token)
                .ConfigureAwait(false);
        }

        private void RequestThreadEntry(HttpListenerContext context)
        {
            try
            {
                var absolutePath = context.Request.Url.AbsolutePath;

                if (context.Request.Url.Segments.Length > 2)
                {
                    absolutePath = context.Request.Url.Segments[0] + context.Request.Url.Segments[1];
                }

                //TODO: this...
                requestHandlerFactory.Get(absolutePath).Handle(context);
            }
            catch (Exception e)
            {
                log.Error(
                    $"Encountered an issue while attempting to either route the request or handle it. Requestor Address - {context.Request.RemoteEndPoint.Address}:{context.Request.RemoteEndPoint.Port}. Request Absolute URL - {context.Request.Url.AbsolutePath}. Request Query - {context.Request.QueryString}.",
                    e);
            }
        }

        private void StartHttpListener()
        {
            try
            {
                log.Debug("Starting Http Listener");
                httpListener.Start();
            }
            catch (HttpListenerException e)
            {
                log.Fatal("Cannot start Httplistener. ", e);
                Environment.Exit(-1);
            }

            Listen();
        }

        private void Listen()
        {
            log.Debug($"Listening to HTTP endpoint - {config.ExposedHttpUrl}");

            while (!CancellationTokenSource.Token.WaitHandle.WaitOne(1))
            {
                HttpListenerContext receivedContent;

                try
                {
                    receivedContent = httpListener.GetContext();
                }
                catch (HttpListenerException e)
                {
                    if (e.ErrorCode != 995)
                    {
                        throw;
                    }

                    log.Info($"Catching HttpListenerException, ErrorCode: {e.ErrorCode}, Message: {e.Message}");

                    continue;
                }

                requestQueue.Add(receivedContent);
            }
        }


        public Task Start()
        {
            log.Info("Run Called");

            return Task.Run(() => StartHttpListener());
        }

        public void Stop()
        {
            log.Debug($"Shutting down.");

            CancellationTokenSource.Cancel();

            try
            {
                httpListener.Stop();
                httpListener.Close();
            }
            catch (Exception)
            {
                //ignored
            }
        }


    }
}