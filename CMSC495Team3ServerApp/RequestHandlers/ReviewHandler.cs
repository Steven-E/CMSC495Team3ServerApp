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
    public class ReviewHandler : SupportedRequestHandlerBase
    {
        private readonly IUserBeerRankingRepo beerRankingRepo;

        public ReviewHandler(ILogger log, IConfigProvider config, IErrorResponseFactory errorResponseFactory,
            IUserBeerRankingRepo beerRankingRepo) : base(log, config, errorResponseFactory)
        {
            this.beerRankingRepo = beerRankingRepo;

            SupportedActions = new Dictionary<HttpMethod, Action<HttpListenerContext, string[]>>();

            SupportedActions.Add(HttpMethod.Get, GetAction);
            SupportedActions.Add(HttpMethod.Post, PostAction);

            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "userId/{ID}", "URL",
                "Review Collection - JSON payload", typeof(ICollection<UserBeerRanking>)));
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "userId/{USERID}/beerId/{BEERID}", "URL",
                "Review Collection - JSON payload", typeof(UserBeerRanking)));

            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "beerId/{ID}", "URL", "Review Collection - Json",
                typeof(ICollection<UserBeerRanking>)));
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "beerId/{BEERID}/userId/{USERID}", "URL",
                "Review Collection - JSON payload", typeof(UserBeerRanking)));

            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "untappdId/{ID}", "URL",
                "Review Collection - JSON payload", typeof(ICollection<UserBeerRanking>)));

            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "breweryDbId/{ID}", "URL",
                "Review Collection - JSON payload", typeof(ICollection<UserBeerRanking>)));

            EndpointDocumentation.Add(new RestDoc("POST", UrlSegment + "add/", "JSON payload", "Status - boolean",
                typeof(bool)));
            EndpointDocumentation.Add(new RestDoc("POST", UrlSegment + "update/", "JSON payload",
                "Updated Review - JSON payload", typeof(UserBeerRanking)));
        }

        protected sealed override Dictionary<HttpMethod, Action<HttpListenerContext, string[]>> SupportedActions
        {
            get;
        }

        public override string UrlSegment => "/reviews/";

        private void GetAction(HttpListenerContext httpListenerContext, string[] route)
        {
            try
            {
                switch (route[0])
                {
                    case "userId":
                        var uId = int.Parse(route[1]);
                        if (route.Length < 3)
                            SendResponse(httpListenerContext, beerRankingRepo.FindAllByUserId(uId));
                        else if (string.Equals(route[2], "beerId") && int.TryParse(route[3], out var oId))
                            SendResponse(httpListenerContext, beerRankingRepo.FindSingleByUserAndBeerId(uId, oId));
                        else
                            goto default;
                        break;
                    case "beerId":
                        var bId = int.Parse(route[1]);
                        if (route.Length < 3)
                            SendResponse(httpListenerContext, beerRankingRepo.FindAllByBeerId(bId));
                        else if (string.Equals(route[2], "userId") && int.TryParse(route[3], out var oId))
                            SendResponse(httpListenerContext, beerRankingRepo.FindSingleByUserAndBeerId(oId, bId));
                        else
                            goto default;
                        break;
                    case "untappdId":
                        var untappdId = int.Parse(route[1]);
                        SendResponse(httpListenerContext, beerRankingRepo.FindAllByUntappdId(untappdId));
                        break;
                    case "breweryDbId":
                        var breweryDbId = int.Parse(route[1]);
                        SendResponse(httpListenerContext, beerRankingRepo.FindAllByBreweryDbId(breweryDbId));
                        break;
                    case "help":
                        SendOKResponseAndPayload(httpListenerContext,
                            JsonConvert.SerializeObject(EndpointDocumentation, Formatting.Indented));
                        break;
                    default:
                        ErrorResponse
                            .Get(HttpStatusCode.BadRequest)
                            .Handle(httpListenerContext,
                                $"Bad Request - No '/rankings/{string.Join("/", route)}/...' exists");
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

        private void PostAction(HttpListenerContext httpListenerContext, string[] route)
        {
            UserBeerRanking ranking;
            try
            {
                var json = ReadJsonContent(httpListenerContext);

                ranking = JsonConvert.DeserializeObject<UserBeerRanking>(json);
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
                        SendResponse(httpListenerContext, beerRankingRepo.Insert(ranking));
                        break;
                    case "update":
                        SendResponse(httpListenerContext, beerRankingRepo.Update(ranking));
                        break;
                    default:
                        ErrorResponse
                            .Get(HttpStatusCode.BadRequest)
                            .Handle(httpListenerContext,
                                $"Bad Request - No '/rankings/{string.Join("/", route)}/...' exists");
                        break;
                }
            }
            catch (Exception e)
            {
                ErrorResponse.Get(HttpStatusCode.InternalServerError).Handle(httpListenerContext, e.Message);
            }
        }
    }
}