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
    public class UserHandler : SupportedRequestHandlerBase
    {
        private readonly IUserInfoRepo userRepo;

        public UserHandler(ILogger log, IConfigProvider config, IErrorResponseFactory errorResponseFactory,
            IUserInfoRepo userRepo) : base(log,
            config, errorResponseFactory)
        {
            this.userRepo = userRepo;

            SupportedActions = new Dictionary<HttpMethod, Action<HttpListenerContext, string[]>>();

            SupportedActions.Add(HttpMethod.Get, GetAction);
            SupportedActions.Add(HttpMethod.Post, PostAction);

            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "userId/{ID}", "URL", "UserInfo - JSON Payload",
                typeof(UserInfo)));
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "untappdId/{ID}", "URL",
                "UserInfo - JSON Payload", typeof(UserInfo)));
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "firstname/{name}", "URL",
                "UserInfo - JSON Payload", typeof(UserInfo)));
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "lastname/{name}", "URL",
                "UserInfo - JSON Payload", typeof(UserInfo)));
            EndpointDocumentation.Add(new RestDoc("GET", UrlSegment + "location/{location}", "URL",
                "UserInfo - JSON Payload", typeof(UserInfo)));
            EndpointDocumentation.Add(new RestDoc("POST", UrlSegment + "add/", "JSON Payload", "UserId - int",
                typeof(UserInfo)));
            EndpointDocumentation.Add(new RestDoc("POST", UrlSegment + "update/", "JSON Payload",
                "JSON Payload - Updated UserInfo", typeof(UserInfo)));
        }

        protected override Dictionary<HttpMethod, Action<HttpListenerContext, string[]>> SupportedActions { get; }
        public override string UrlSegment => "/user/";

        private void GetAction(HttpListenerContext httpListenerContext, string[] route)
        {
            try
            {
                switch (route[0])
                {
                    case "userId":
                        var userId = int.Parse(route[1]);
                        SendResponse(httpListenerContext, userRepo.FindById(userId));
                        break;
                    case "untappdId":
                        var uId = int.Parse(route[1]);
                        SendResponse(httpListenerContext, userRepo.FindByUntappdId(uId));
                        break;
                    case "firstname":
                        SendResponse(httpListenerContext, userRepo.FindByFirstName(route[1]));
                        break;
                    case "lastname":
                        SendResponse(httpListenerContext, userRepo.FindByLastName(route[1]));
                        break;
                    case "location":
                        SendResponse(httpListenerContext,
                            userRepo.FindByLocation(route[1].Replace(";", string.Empty).Replace("\"", string.Empty)
                                .Replace("?", string.Empty)));
                        break;
                    case "help":
                        SendOKResponseAndPayload(httpListenerContext,
                            JsonConvert.SerializeObject(EndpointDocumentation, Formatting.Indented));
                        break;
                    default:
                        ErrorResponse
                            .Get(HttpStatusCode.BadRequest)
                            .Handle(httpListenerContext,
                                $"Bad Request - No '/user/{string.Join("/", route)}/...' exists");
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
            UserInfo user;

            try
            {
                var json = ReadJsonContent(httpListenerContext);

                user = JsonConvert.DeserializeObject<UserInfo>(json);
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
                        SendResponse(httpListenerContext, userRepo.Insert(user));
                        break;
                    case "update":
                        SendResponse(httpListenerContext, userRepo.Update(user));
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