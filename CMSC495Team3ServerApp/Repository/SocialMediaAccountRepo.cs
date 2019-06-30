using System;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Models.App;
using CMSC495Team3ServerApp.Provider;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.Repository
{
    public class SocialMediaAccountRepo : RepoBase<SocialMediaAccount>, ISocialMediaAccountRepo
    {
        public SocialMediaAccountRepo(ILogger logger, IConfigProvider configProvider) : base(logger, configProvider)
        {
        }

        public void Insert(SocialMediaAccount appObj, int userId)
        {
            const string sql =
                "INSERT INTO SocialMediaAccount " +
                "(User_FK, Vender_FK, AccountId) VALUES " +
                "(@User, @Vender, \"@AccountId\")";

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    connection.Execute(sql, new
                    {
                        User = userId,
                        Vender = appObj.Vendor.Vendor,
                        appObj.AccountId
                    });
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not insert '{JsonConvert.SerializeObject(appObj)}.'", e);
            }
        }

        public override TransactionResult<SocialMediaAccount> Update(SocialMediaAccount appObj, int userId)
        {
            const string sql =
                "UPDATE SocialMediaAccount SET " +
                "AccountId = @AccountId " +
                "WHERE User_FK = @UserId " +
                "AND Vendor_FK = @Vendor";

            var retVal = new TransactionResult<SocialMediaAccount>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    connection.Execute(sql, new
                    {
                        UserId = userId,
                        appObj.AccountId,
                        appObj.Vendor.Vendor
                    });
                }

                retVal.Success = true;
                retVal.Data = appObj;
            }
            catch (Exception e)
            {
                Log.Error($"Could not update '{JsonConvert.SerializeObject(appObj)}.'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public override TransactionResult<SocialMediaAccount> Update(SocialMediaAccount appObj)
        {
            throw new NotImplementedException();
        }
    }

    public interface ISocialMediaAccountRepo
    {
        void Insert(SocialMediaAccount appObj, int userId);
        TransactionResult<SocialMediaAccount> Update(SocialMediaAccount appObj, int userId);
    }
}