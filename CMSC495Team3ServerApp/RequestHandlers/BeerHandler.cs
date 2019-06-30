using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Models;
using CMSC495Team3ServerApp.Models.App;
using CMSC495Team3ServerApp.Provider;
using CMSC495Team3ServerApp.Repository;
using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public class BeerHandler : SupportedRequestHandlerBase
    {
        private readonly IBeerRepo beerRepo;

        public BeerHandler(ILogger log, IConfigProvider config, IErrorResponseFactory errorResponseFactory,
            IBeerRepo beerRepo) : base(log, config, errorResponseFactory)
        {
            this.beerRepo = beerRepo;

            SupportedActions = new Dictionary<HttpMethod, Action<HttpListenerContext, string[]>>();

            SupportedActions.Add(HttpMethod.Get, GetAction);
            SupportedActions.Add(HttpMethod.Post, PostAction);

            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "untappdId/{ID}", "URL", "JSON payload", typeof(Beer)));
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "beerId/{ID}", "URL", "JSON payload", typeof(Beer)));
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "breweryId/{ID}", "URL", "JSON payload", typeof(ICollection<Beer>)));

            EndpointDocumentation.Add(new RestDoc("POST", UrlSegment + "add/", "JSON payload", "BeerId - int", typeof(Beer)));
            EndpointDocumentation.Add(new RestDoc("POST", UrlSegment + "update/", "JSON payload", "Updated Beer - JSON payload", typeof(Beer)));
        }

        protected sealed override Dictionary<HttpMethod, Action<HttpListenerContext, string[]>> SupportedActions { get; }

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

            try
            {
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
            catch (Exception e)
            {
                ErrorResponse.Get(HttpStatusCode.InternalServerError).Handle(httpListenerContext, e.Message);
            }

        }

        // Finds by App.BeerId, UntappdId, App.BreweryId
        private void GetAction(HttpListenerContext httpListenerContext, string[] route)
        {
            try
            {


                switch (route[0])
                {
                    case "untappdId":
                        var uId = int.Parse(route[1]);
                        SendResponse(httpListenerContext, beerRepo.FindUntappdId(uId));
                        break;
                    case "beerId":
                        var id = int.Parse(route[1]);
                        SendResponse(httpListenerContext, beerRepo.FindById(id));
                        break;
                    case "breweryId":
                        var bId = int.Parse(route[1]);
                        SendResponse(httpListenerContext, beerRepo.FindByBreweryId(bId));
                        break;
                    case "help":
                        SendOKResponseAndPayload(httpListenerContext, JsonConvert.SerializeObject(EndpointDocumentation, Formatting.Indented));
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
                if (e is ArgumentNullException || e is ArgumentException || e is FormatException ||
                    e is OverflowException)
                    ErrorResponse.Get(HttpStatusCode.BadRequest).Handle(httpListenerContext, "Badly formed Id");
                else
                    ErrorResponse.Get(HttpStatusCode.InternalServerError).Handle(httpListenerContext, e.Message);
            }
        }
    }
}