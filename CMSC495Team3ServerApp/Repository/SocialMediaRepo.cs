﻿using System;
using System.Linq;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Models.App;
using CMSC495Team3ServerApp.Provider;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.Repository
{
    public class SocialMediaRepo : RepoBase<SocialMedia>, ISocialMediaRepo
    {
        public SocialMediaRepo(ILogger logger, IConfigProvider configProvider) : base(logger, configProvider)
        {
        }

        public void Insert(SocialMedia appObj)
        {
            const string sql =
                "INSERT INTO SocialMedia (Vender, BaseUrl, ApiUrl) " +
                "VALUES ('@Vender', '@BaseUrl', '@ApiUrl')";

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    connection.Query(sql, new
                                          {
                                              Vender = appObj.Vendor,
                                              appObj.BaseUrl,
                                              appObj.ApiUrl
                                          });
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not insert '{JsonConvert.SerializeObject(appObj)}.'", e);
            }
        }


        public TransactionResult<SocialMedia> Find(string vendorId)
        {
            const string sql = "SELECT * FROM  SocialMedia WHERE Vendor = @VendorId";

            var retVal = new TransactionResult<SocialMedia>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<SocialMedia>(sql, new
                                                                     {
                                                                         VendorId = vendorId
                                                                     }).FirstOrDefault();

                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not perform FIND using Vender Id - " +
                          $"'{vendorId}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public override TransactionResult<SocialMedia> Update(SocialMedia appObj)
        {
            throw new NotImplementedException();
        }

        public override TransactionResult<SocialMedia> Update(SocialMedia appObj, int referenceKey)
        {
            throw new NotImplementedException();
        }
    }

    public interface ISocialMediaRepo
    {
        void Insert(SocialMedia appObj);

        TransactionResult<SocialMedia> Find(string vendorId);
    }
}