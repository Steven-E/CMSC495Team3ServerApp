using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using CMSC495Team3ServerApp.Adapters;
using CMSC495Team3ServerApp.ApiClients;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Models;
using CMSC495Team3ServerApp.Models.Untappd;
using CMSC495Team3ServerApp.Provider;
using CMSC495Team3ServerApp.Repository;
using Newtonsoft.Json;
using Beer = CMSC495Team3ServerApp.Models.App.Beer;
using Brewery = CMSC495Team3ServerApp.Models.App.Brewery;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public class BeerHandler : SupportedRequestHandlerBase
    {
        private readonly IBeerRepo beerRepo;
        private readonly IBreweryRepo breweryRepo;
        private readonly IUntappdApiClient client;

        public BeerHandler(ILogger log, IConfigProvider config,
            IErrorResponseFactory errorResponseFactory,
            IBeerRepo beerRepo, IBreweryRepo breweryRepo, IUntappdApiClient client) : base(log,
            config,
            errorResponseFactory)
        {
            this.beerRepo = beerRepo;
            this.breweryRepo = breweryRepo;

            this.client = client;

            SupportedActions.Add(HttpMethod.Get, GetAction);
            SupportedActions.Add(HttpMethod.Post, PostAction);

            SetupDocumentation();
        }

        public override string UrlSegment => "/beer/";

        private void SetupDocumentation()
        {
            //GETs
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "untappdId/{ID}", "URL",
                "JSON payload",
                typeof(Beer)));
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "beerId/{ID}", "URL",
                "JSON payload",
                typeof(Beer)));
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "breweryId/{ID}", "URL",
                "JSON payload",
                typeof(ICollection<Beer>)));

            //POSTs
            EndpointDocumentation.Add(new RestDoc("POST", UrlSegment + "add/", "JSON payload",
                "BeerId - int",
                typeof(Beer)));
            EndpointDocumentation.Add(new RestDoc("POST", UrlSegment + "update/", "JSON payload",
                "Updated Beer - JSON payload", typeof(Beer)));
        }

        // Need to do Update and Insert
        private void PostAction(HttpListenerContext httpListenerContext, string[] route)
        {
            string json;
            Beer beer;
            try
            {
                json = ReadJsonContent(httpListenerContext);

                beer = JsonConvert.DeserializeObject<Beer>(json);
            }
            catch (Exception e)
            {
                ErrorResponse
                    .Get(HttpStatusCode.BadRequest)
                    .Handle(httpListenerContext,
                        $"Malformed or bad data provided in Json Body. Details - {e.Message}");
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
                            .Handle(httpListenerContext,
                                $"Bad Request - No '/beer/{route[0]}/...' exists");
                        break;
                }
            }
            catch (Exception e)
            {
                ErrorResponse.Get(HttpStatusCode.InternalServerError).Handle(httpListenerContext,
                    $"Route - '{string.Join("/", route)}'," +
                    $" Content - '{json}'");
            }
        }

        // Finds by App.BeerId, UntappdId, App.BreweryId
        private void GetAction(HttpListenerContext httpListenerContext, string[] route)
        {
            try
            {
                var queryString = httpListenerContext.Request.QueryString;

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
                    case "search":
                        var name = queryString["name"];
                        var dbResults = beerRepo.FindByName(name);

                        //no records matching exist in DB
                        if (!dbResults.Success || dbResults.Data.Count == 0)
                        {
                            //try find in Untappd
                            var requestWrapper =
                                new RequestWrapper
                                {
                                    HttpMethod = HttpMethod.Get,
                                    RelativePath = "search/beer",
                                    QueryParameters =
                                        new List<KeyValuePair<string, string>>
                                        {
                                            new KeyValuePair<string, string>("q",
                                                name),
                                            new KeyValuePair<string, string>(
                                                "sort",
                                                "name")
                                        }
                                };

                            var clientResults = client.Get(requestWrapper);

                            if (clientResults.Success && clientResults.Data.IsSuccess)
                            {
                                var uResponse =
                                    JsonConvert.DeserializeObject<UntappdResponse>(
                                            clientResults.Data.Content.ToString())
                                        as UntappdResponse;


                                if (uResponse.Meta.Code == (int) HttpStatusCode.OK)
                                {
                                    var contentResponse =
                                        JsonConvert.DeserializeObject<BeerSearchResponse>(
                                                uResponse.Response.ToString())
                                            as
                                            BeerSearchResponse;

                                    if (contentResponse.Found > 0)
                                    {
                                        var insertedResults =
                                            new TransactionResult<
                                                ICollection<TransactionResult<Brewery>>>();
                                        insertedResults.Data =
                                            new List<TransactionResult<Brewery>>();

                                        foreach (var item in contentResponse.Beers.Items)
                                        {
                                            var appBrewery = UntappdToAppAdapter.Get(item.Brewery);
                                            var appBeer = UntappdToAppAdapter.Get(item.Beer);
                                            appBrewery.Beers.Add(appBeer);

                                            insertedResults.Data.Add(breweryRepo.AddOrUpdate(
                                                appBrewery));
                                        }

                                        insertedResults.Success =
                                            insertedResults.Data.All(x => x.Success);

                                        SendResponse(httpListenerContext, insertedResults);
                                        break;
                                    }
                                }

                                SendResponse(httpListenerContext,
                                    new TransactionResult<UntappdResponse>
                                    {
                                        Data = uResponse,
                                        Success = uResponse.Meta.Code == (int) HttpStatusCode.OK
                                    });
                                break;
                            }

                            SendResponse(httpListenerContext, clientResults);
                            break;
                        }

                        SendResponse(httpListenerContext, dbResults);

                        break;
                    case "help":
                        SendOKResponseAndPayload(httpListenerContext,
                            JsonConvert.SerializeObject(EndpointDocumentation,
                                Formatting.Indented));
                        break;
                    default:
                        ErrorResponse
                            .Get(HttpStatusCode.BadRequest)
                            .Handle(httpListenerContext,
                                $"Bad Request - No '/beer/{route[0]}/...' exists");
                        break;
                }
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException || e is ArgumentException || e is FormatException ||
                    e is OverflowException)
                    ErrorResponse.Get(HttpStatusCode.BadRequest)
                                 .Handle(httpListenerContext, "Badly formed Id");
                else
                    ErrorResponse.Get(HttpStatusCode.InternalServerError)
                                 .Handle(httpListenerContext,
                                     $"Route - '{string.Join("/", route)}'");
            }
        }
    }
}