using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Models.App;
using CMSC495Team3ServerApp.Provider;
using CMSC495Team3ServerApp.Repository;
using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public class BeerHandler : RequestHandlerBase
    {
        private readonly IBeerRepo beerRepo;

        public BeerHandler(ILogger log, IConfigProvider config, IErrorResponseFactory errorResponseFactory,
            IBeerRepo beerRepo) : base(log, config, errorResponseFactory)
        {
            this.beerRepo = beerRepo;

            SupportedActions = new Dictionary<HttpMethod, Action<HttpListenerContext, string[]>>();

            SupportedActions.Add(HttpMethod.Get, GetAction);
            SupportedActions.Add(HttpMethod.Post, PostAction);

            //SupportedRoutes.Add("untappdId", );
        }

        protected override Dictionary<HttpMethod, Action<HttpListenerContext, string[]>> SupportedActions { get; }

        public override string UrlSegment => "/beer/";

        // Need to do Update and Insert
        private void PostAction(HttpListenerContext httpListenerContext, string[] route)
        {
            Beer beer;
            try
            {
                var json = ReadJsonContent(httpListenerContext);

                beer = JsonConvert.DeserializeObject<Beer>(json);
            }
            catch (Exception e)
            {
                ErrorResponse
                    .Get(HttpStatusCode.BadRequest)
                    .Handle(httpListenerContext, $"Malformed or bad data provided in Json Body. Details - {e.Message}");
                return;
            }

            switch (route[0])
            {
                case "add":
                    SendResponse(httpListenerContext, beerRepo.Insert(beer));
                    break;
                case "update":
                    SendResponse(httpListenerContext, beerRepo.Update(beer));
                    break;
                default:
                    ErrorResponse
                        .Get(HttpStatusCode.BadRequest)
                        .Handle(httpListenerContext, $"Bad Request - No '/beer/{route[0]}/...' exists");
                    break;
            }
        }

        // Finds by App.BeerId, UntappdId, App.BreweryId
        private void GetAction(HttpListenerContext httpListenerContext, string[] route)
        {
            try
            {
                var id = int.Parse(route[1]);

                switch (route[0])
                {
                    case "untappdId":
                        SendResponse(httpListenerContext, beerRepo.FindUntappdId(id));
                        break;
                    case "beerId":
                        SendResponse(httpListenerContext, beerRepo.FindById(id));
                        break;
                    case "breweryId":
                        SendResponse(httpListenerContext, beerRepo.FindByBreweryId(id));
                        break;
                    default:
                        ErrorResponse
                            .Get(HttpStatusCode.BadRequest)
                            .Handle(httpListenerContext, $"Bad Request - No '/beer/{route[0]}/...' exists");
                        break;
                }
            }
            catch (Exception e)
            {
                ErrorResponse.Get(HttpStatusCode.InternalServerError).Handle(httpListenerContext, e.Message);
            }
        }

        protected override void ProcessRequest(HttpListenerContext httpListenerContext, HttpMethod method)
        {
            //var route = httpListenerContext.Request.Url.AbsolutePath.Remove(0, UrlSegment.Length);

            var route = httpListenerContext.Request.Url.AbsolutePath.Remove(0, UrlSegment.Length)
                .Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);

            SupportedActions[method](httpListenerContext, route);
        }
    }

    //405 Method Not Allowed
    public class MethodNotAllowedHandler : ErrorResponseHandlerBase
    {
        public MethodNotAllowedHandler(ILogger log, IConfigProvider config) : base(log, config)
        {
        }

        protected override string Description => "Method not allowed";
        public override HttpStatusCode StatusCode => HttpStatusCode.MethodNotAllowed;
    }

    //500 Internal Server Error
    public class ServerErrorHandler : ErrorResponseHandlerBase
    {
        public ServerErrorHandler(ILogger log, IConfigProvider config) : base(log, config)
        {
        }

        protected override string Description => "Internal Server Error";
        public override HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
    }

    //400 Bad Request
    public class BadRequestHandler : ErrorResponseHandlerBase
    {
        public BadRequestHandler(ILogger log, IConfigProvider config) : base(log, config)
        {
        }

        protected override string Description => "Bad Request";
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    }

    //404 Not Found
    public class NotFoundHandler : ErrorResponseHandlerBase
    {
        public NotFoundHandler(ILogger log, IConfigProvider config) : base(log, config)
        {
        }

        protected override string Description => "Requested resource does not exist or cannot be found.";
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    }
}