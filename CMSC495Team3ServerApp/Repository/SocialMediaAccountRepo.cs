using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ISocialMediaRepo socialMediaVendorRepo;

        public SocialMediaAccountRepo(ILogger logger, IConfigProvider configProvider,
            ISocialMediaRepo socialMediaVendorRepo) : base(logger, configProvider)
        {
            this.socialMediaVendorRepo = socialMediaVendorRepo;
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
                    connection.Execute(sql,
                        new
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
                    connection.Execute(sql,
                        new
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

        public TransactionResult<ICollection<SocialMediaAccount>> FindByUserId(int userId)
        {
            const string sql = "SELECT * FROM SocialMediaAccount WHERE " +
                               "User_FK = @userId";

            var retVal = new TransactionResult<ICollection<SocialMediaAccount>>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<SocialMediaAccount>(sql,
                        new
                        {
                            userId
                        }).ToList();
                }

                retVal.Success = true;

                foreach (var account in retVal.Data.Where(a => a != null))
                {
                    var vendorRetVal = socialMediaVendorRepo.Find(account.VendorName);

                    account.Vendor = vendorRetVal.Data;

                    if (!vendorRetVal.Success)
                    {
                        retVal.Success = false;
                        retVal.Details = vendorRetVal.Details;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND in SocialMediaAccount Table using UserId - '{userId}'", e);

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

        TransactionResult<ICollection<SocialMediaAccount>> FindByUserId(int userId);
    }
}