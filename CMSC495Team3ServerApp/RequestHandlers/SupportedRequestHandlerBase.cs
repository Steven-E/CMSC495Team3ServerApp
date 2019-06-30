using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Models;
using CMSC495Team3ServerApp.Provider;
using CMSC495Team3ServerApp.Repository;
using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.RequestHandlers
{
    public abstract class SupportedRequestHandlerBase : ISupportedRequestHandler
    {
        protected readonly IConfigProvider Config;
        protected readonly IErrorResponseFactory ErrorResponse;
        protected readonly ILogger Log;

        protected SupportedRequestHandlerBase(ILogger log, IConfigProvider config,
            IErrorResponseFactory errorResponseFactory) //, ISessionManager sessionManager)
        {
            Config = config;

            Log = log;

            ErrorResponse = errorResponseFactory;

            //SessionManager = sessionManager;
        }

        protected abstract Dictionary<HttpMethod, Action<HttpListenerContext, string[]>> SupportedActions { get; }

        protected List<RestDoc> EndpointDocumentation = new List<RestDoc>();

        //protected readonly string ConnectionString;


        //protected readonly ISessionManager SessionManager;

        public abstract string UrlSegment { get; }

        public void Handle(HttpListenerContext httpListenerContext)
        {
            Log.Info($"Received '{httpListenerContext.Request.HttpMethod}' from client - " +
                     $"{httpListenerContext.Request.RemoteEndPoint.Address}:{httpListenerContext.Request.RemoteEndPoint.Port}");

            var requestMethodType = new HttpMethod(httpListenerContext.Request.HttpMethod);

            if (!SupportedActions.ContainsKey(requestMethodType))
            {
                ErrorResponse.Get(HttpStatusCode.BadRequest).Handle(httpListenerContext);
                return;
            }

            ProcessRequest(httpListenerContext, requestMethodType);
        }

        //protected abstract void ProcessRequest(HttpListenerContext httpListenerContext, HttpMethod method);
        protected virtual void ProcessRequest(HttpListenerContext httpListenerContext, HttpMethod method)
        {
            var route = httpListenerContext.Request.Url.AbsolutePath.Remove(0, UrlSegment.Length)
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            SupportedActions[method](httpListenerContext, route);
        }

        protected void SendOKResponseAndPayload(HttpListenerContext httpListenerContext, string responseJson)
        {
            var responseBinary = Encoding.UTF8.GetBytes(responseJson);

            httpListenerContext.Response.ContentEncoding = Encoding.UTF8;
            httpListenerContext.Response.ContentType = "application/json";
            httpListenerContext.Response.StatusCode = (int) HttpStatusCode.OK;
            httpListenerContext.Response.ContentLength64 = responseBinary.Length;
            httpListenerContext.Response.OutputStream.Write(responseBinary, 0, responseBinary.Length);
            httpListenerContext.Response.OutputStream.Close();
        }

        protected void SendOKResponseAndPayload<T>(HttpListenerContext httpListenerContext,
            TransactionResult<T> transactionResult)
        {
            SendOKResponseAndPayload(httpListenerContext, JsonConvert.SerializeObject(transactionResult.Data));
        }

        protected void SendErrorResponseAndDetails<T>(HttpListenerContext httpListenerContext,
            TransactionResult<T> transactionResult)
        {
            ErrorResponse.Get(HttpStatusCode.InternalServerError).Handle(httpListenerContext,
                JsonConvert.SerializeObject(transactionResult.Details));
        }

        protected void SendResponse<T>(HttpListenerContext httpListenerContext, TransactionResult<T> transactionResult)
        {
            if (transactionResult.Success)
                SendOKResponseAndPayload(httpListenerContext, transactionResult);
            else
                SendErrorResponseAndDetails(httpListenerContext, transactionResult);
        }

        protected string ReadJsonContent(HttpListenerContext httpListenerContext)
        {
            string requestText;

            using (var reader = new StreamReader(httpListenerContext.Request.InputStream,
                httpListenerContext.Request.ContentEncoding))
            {
                requestText = reader.ReadToEnd();
            }

            return requestText;
        }

        //protected bool IsAuthorized(HttpListenerContext httpListenerContext, out string bearerToken)
        //{
        //    bearerToken = null;

        //    var authenticationHeader = httpListenerContext.Request.Headers["Authorization"];

        //    if (authenticationHeader == null)
        //    {
        //        return false;
        //    }

        //    if (!authenticationHeader.StartsWith("Bearer"))
        //    {
        //        return false;
        //    }

        //    bearerToken = authenticationHeader.Substring("Bearer ".Length).Trim();

        //    return SessionManager.IsValidSession(bearerToken);
        //}
    }
}