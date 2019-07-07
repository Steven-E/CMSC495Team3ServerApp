using System;
using System.Collections.Generic;
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
using Brewery = CMSC495Team3ServerApp.Models.App.Brewery;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public class BreweryHandler : SupportedRequestHandlerBase
    {
        private readonly IBreweryRepo breweryRepo;
        private readonly IUntappdApiClient client;

        public BreweryHandler(ILogger log, IConfigProvider config, IErrorResponseFactory errorResponseFactory,
            IBreweryRepo breweryRepo, IUntappdApiClient client) : base(log, config, errorResponseFactory)
        {
            this.breweryRepo = breweryRepo;

            this.client = client;

            SupportedActions.Add(HttpMethod.Get, GetAction);
            SupportedActions.Add(HttpMethod.Post, PostAction);

            SetupDocumentation();
        }

        public override string UrlSegment => "/brewery/";

        private void SetupDocumentation()
        {
            //EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "", "", "", typeof()));
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "id/{ID}?GetBeers={bool}", "URL",
                "Brewery - JSON payload", typeof(Brewery)));
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "untappdId/{ID}?GetBeers={bool}", "URL",
                "Brewery - JSON payload", typeof(Brewery)));
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "breweryDbId/{ID}?GetBeers={bool}", "URL",
                "Brewery - JSON payload", typeof(Brewery)));
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "all?GetBeers={bool}", "URL",
                "Brewery Collection - JSON payload", typeof(ICollection<Brewery>)));

            //EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "", "", "", typeof()));
            EndpointDocumentation.Add(new RestDoc("POST", UrlSegment + "add", "JSON payload", "Brewery - JSON payload",
                typeof(Brewery)));
            EndpointDocumentation.Add(new RestDoc("POST", UrlSegment + "update", "JSON payload",
                "Brewery - JSON payload", typeof(Brewery)));
        }

        private void GetAction(HttpListenerContext context, string[] route)
        {
            try
            {
                var queryString = context.Request.QueryString;

                var getBeers = false;
                var id = 0;

                try
                {
                    if (!string.Equals("help", route[0]))
                    {
                        if (!bool.TryParse(queryString["GetBeers"], out getBeers))
                        {
                            SendBadRequest(context);
                            return;
                        }

                        if (!string.Equals("all", route[0]) && !int.TryParse(route[1], out id))
                        {
                            SendBadRequest(context);
                            return;
                        }
                    }
                }
                catch (Exception)
                {
                    SendBadRequest(context);
                    return;
                }

                switch (route[0])
                {
                    case "id":
                        SendResponse(context, breweryRepo.FindById(id, getBeers));
                        break;
                    case "untappdId":

                        var dbResults = breweryRepo.FindByUntappdId(id, getBeers);

                        //client
                        if (!dbResults.Success || dbResults.Data == null)
                        {
                            var requestWrapper = new RequestWrapper()
                            {
                                HttpMethod = HttpMethod.Get,
                                RelativePath = $"brewery/info/{id}",
                            };
                            
                            requestWrapper.QueryParameters = new List<KeyValuePair<string, string>>()
                            {
                                new KeyValuePair<string, string>("compact", (!getBeers).ToString().ToLower())
                            };


                            var clientResults = client.Get(requestWrapper);

                            if (clientResults.Success && clientResults.Data.IsSuccess)
                            {
                                var uResponse = JsonConvert.DeserializeObject<UntappdResponse>(clientResults.Data.Content.ToString())
                                    as UntappdResponse;

                                if (uResponse.Meta.Code == (int) HttpStatusCode.OK)
                                {
                                    var breweryResponse =
                                        JsonConvert.DeserializeObject<Models.Untappd.Brewery>(uResponse.Response.brewery
                                            .ToString()) as Models.Untappd.Brewery;

                                    //brewery.beer_list.items[].beer
                                    //uResponse.Response.brewery.beer_list.items[0].beer
                                    //var breweryBeers = new List<Beer>();
                                    var breweryBeers = new List<Models.App.Beer>();

                                    foreach (dynamic item in uResponse.Response.brewery.beer_list.items)
                                    {
                                        var uBeer = JsonConvert.DeserializeObject<Beer>(item.beer.ToString()) as Beer;

                                        breweryBeers.Add( UntappdToAppAdapter.Get(uBeer));
                                    }

                                    var breweryObj = UntappdToAppAdapter.Get(breweryResponse);

                                    breweryObj.Beers = breweryBeers;

                                    SendResponse(context, breweryRepo.AddOrUpdate(breweryObj));
                                }
                            }
                        }
                        

                        SendResponse(context, dbResults);
                        break;
                    case "breweryDbId":
                        SendResponse(context, breweryRepo.FindBreweryDbId(id, getBeers));
                        break;
                    case "all":
                        SendResponse(context, breweryRepo.FindAllBreweries(getBeers));
                        break;
                    case "help":
                        SendOKResponseAndPayload(context, JsonConvert.SerializeObject(EndpointDocumentation));
                        break;
                    default:
                        ErrorResponse
                            .Get(HttpStatusCode.BadRequest)
                            .Handle(context, $"Bad Request - No '/beer/{route[0]}/...' exists");
                        break;
                }
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException || e is ArgumentException || e is FormatException ||
                    e is OverflowException)
                    ErrorResponse.Get(HttpStatusCode.BadRequest).Handle(context, "Badly formed Id");
                else
                    ErrorResponse.Get(HttpStatusCode.InternalServerError).Handle(context, e.Message);
            }
        }

        private void PostAction(HttpListenerContext context, string[] route)
        {
            string json;
            Brewery brewery;
            try
            {
                json = ReadJsonContent(context);

                brewery = JsonConvert.DeserializeObject<Brewery>(json);
            }
            catch (Exception e)
            {
                ErrorResponse
                    .Get(HttpStatusCode.BadRequest)
                    .Handle(context, $"Malformed or bad data provided in Json Body. Details - {e.Message}");
                return;
            }

            try
            {
                switch (route[0])
                {
                    case "add":
                        SendResponse(context, breweryRepo.Insert(brewery));
                        break;
                    case "update":
                        SendResponse(context, breweryRepo.Update(brewery));
                        break;
                    default:
                        ErrorResponse
                            .Get(HttpStatusCode.BadRequest)
                            .Handle(context,
                                $"Bad Request - No '/rankings/{string.Join("/", route)}/...' exists");
                        break;
                }
            }
            catch (Exception)
            {
                ErrorResponse.Get(HttpStatusCode.InternalServerError).Handle(context,
                    $"Route - '{string.Join("/", route)}'," +
                    $" Content - '{json}'");
            }
        }
    }
}