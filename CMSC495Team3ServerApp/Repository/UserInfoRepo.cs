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
    public class UserInfoRepo : RepoBase<UserInfo>, IUserInfoRepo
    {
        private readonly IUserBeerRankingRepo rankRepo;
        private readonly ISocialMediaAccountRepo socialAccRepo;


        public UserInfoRepo(ILogger logger, IConfigProvider configProvider, IUserBeerRankingRepo rankRepo,
            ISocialMediaAccountRepo socialAccRepo) : base(logger, configProvider)
        {
            this.rankRepo = rankRepo;
            this.socialAccRepo = socialAccRepo;
        }

        public TransactionResult<int> Insert(UserInfo appObj)
        {
            const string sql =
                "INSERT INTO UserInfo (UserName, Password, UserEmail, " +
                " FirstName, LastName, Location, UntappdId) VALUES " +
                "(@UserName, @Password, @UserEmail, @FirstName, @LastName, @Location, @UntappdId);" +
                "SELECT LAST_INSERT_ID();";
            var retVal = new TransactionResult<int>();

            try
            {
                //int userId;

                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<int>(sql, new
                    {
                        appObj.UserName,
                        appObj.Password,
                        appObj.UserEmail,
                        appObj.FirstName,
                        appObj.LastName,
                        appObj.Location,
                        appObj.UntappdId
                    }).Single();

                    //if appObject has nested values....
                }

                //TODO: do better than these loops. Add to sql above using dapper.
                appObj.BeerRankings.ForEach(r => r.User_FK = retVal.Data);

                foreach (var ranking in appObj.BeerRankings) rankRepo.Insert(ranking);

                foreach (var account in appObj.SocialAccounts) socialAccRepo.Insert(account, retVal.Data);

                retVal.Success = true;

            }
            catch (Exception e)
            {
                Log.Error($"Could not insert '{JsonConvert.SerializeObject(appObj)}.'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public override TransactionResult<UserInfo> Update(UserInfo appObj)
        {
            const string sql = "UPDATE UserInfo SET " +
                               "UserName = @UserName " +
                               "UserEmail = @UserEmail " +
                               "FirstName = @FirstName " +
                               "LastName = @LastName " +
                               "Location = @Location " +
                               "UntappdId = @UntappdId " +
                               "WHERE UserId = @UserId;";

            var retVal = new TransactionResult<UserInfo>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    connection.Execute(sql, new
                    {
                        appObj.UserName,
                        appObj.UserEmail,
                        appObj.FirstName,
                        appObj.LastName,
                        appObj.Location,
                        appObj.UntappdId,
                        appObj.UserId
                    });
                }

                var userRankingCollection = new List<TransactionResult<UserBeerRanking>>();

                appObj.BeerRankings.ForEach(r => r.User_FK = appObj.UserId);

                foreach (var ranking in appObj.BeerRankings)
                    userRankingCollection.Add(rankRepo.Update(ranking));

                var socialAccountsCollection = new List<TransactionResult<SocialMediaAccount>>();

                foreach (var account in appObj.SocialAccounts)
                    socialAccountsCollection.Add(socialAccRepo.Update(account, appObj.UserId));

                appObj.BeerRankings = userRankingCollection.Where(e => e.Success).Select(e => e.Data).ToList();
                appObj.SocialAccounts = socialAccountsCollection.Where(e => e.Success).Select(e => e.Data).ToList();

                retVal.Data = appObj;
                retVal.Success = true;
            }
            catch (Exception e)
            {
                Log.Error($"Could not update '{JsonConvert.SerializeObject(appObj)}.'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<UserInfo> FindById(int id)
        {
            const string sql = "SELECT * FROM UserInfo WHERE " +
                       "UserId = @Id";

            var retVal = new TransactionResult<UserInfo>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<UserInfo>(sql, new
                    {
                        id
                    }).FirstOrDefault();

                }

                if (retVal.Data != null)
                {
                    var accounts = socialAccRepo.FindByUserId(retVal.Data.UserId).Data;

                    retVal.Data.SocialAccounts = accounts?.ToList();

                    var rankings = rankRepo.FindAllByUserId(retVal.Data.UserId).Data;

                    retVal.Data.BeerRankings = rankings?.ToList();
                }

                retVal.Success = true;
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using UserId - '{id}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<UserInfo> FindByUntappdId(int id)
        {
            const string sql = "SELECT * FROM UserInfo WHERE " +
                               "UntappdId = @Id";

            var retVal = new TransactionResult<UserInfo>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<UserInfo>(sql, new
                    {
                        id
                    }).FirstOrDefault();
                }

                if (retVal.Data != null)
                {
                    var accounts = socialAccRepo.FindByUserId(retVal.Data.UserId).Data;

                    retVal.Data.SocialAccounts = accounts?.ToList();

                    var rankings = rankRepo.FindAllByUserId(retVal.Data.UserId).Data;

                    retVal.Data.BeerRankings = rankings?.ToList();
                }

                retVal.Success = true;
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using UntappdId - '{id}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<ICollection<UserInfo>> FindByFirstName(string firstName)
        {
            const string sql = "SELECT * FROM UserInfo WHERE " +
                               "FirstName = @firstName";

            var retVal = new TransactionResult<ICollection<UserInfo>>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<UserInfo>(sql, new
                    {
                        firstName
                    }).ToList();
                }

                foreach (var userInfo in retVal.Data)
                {
                    if (userInfo == null) continue;

                    var accounts = socialAccRepo.FindByUserId(userInfo.UserId).Data;

                    userInfo.SocialAccounts = accounts?.ToList();

                    var rankings = rankRepo.FindAllByUserId(userInfo.UserId).Data;

                    userInfo.BeerRankings = rankings?.ToList();
                }

                retVal.Success = true;
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using FirstName - '{firstName}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<ICollection<UserInfo>> FindByLastName(string lastName)
        {
            const string sql = "SELECT * FROM UserInfo WHERE " +
                               "LastName = @lastName";

            var retVal = new TransactionResult<ICollection<UserInfo>>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<UserInfo>(sql, new
                    {
                        lastName
                    }).ToList();
                }

                foreach (var userInfo in retVal.Data)
                {
                    if (userInfo == null) continue;

                    var accounts = socialAccRepo.FindByUserId(userInfo.UserId).Data;

                    userInfo.SocialAccounts = accounts?.ToList();

                    var rankings = rankRepo.FindAllByUserId(userInfo.UserId).Data;

                    userInfo.BeerRankings = rankings?.ToList();
                }

                retVal.Success = true;
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using LastName - '{lastName}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<ICollection<UserInfo>> FindByLocation(string location)
        {
            const string sql = "SELECT * FROM UserInfo WHERE " +
                               "Location = @location";

            var retVal = new TransactionResult<ICollection<UserInfo>>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<UserInfo>(sql, new
                    {
                        location
                    }).ToList();
                }

                foreach (var userInfo in retVal.Data)
                {
                    if (userInfo == null) continue;

                    var accounts = socialAccRepo.FindByUserId(userInfo.UserId).Data;

                    userInfo.SocialAccounts = accounts?.ToList();

                    var rankings = rankRepo.FindAllByUserId(userInfo.UserId).Data;

                    userInfo.BeerRankings = rankings?.ToList();
                }

                retVal.Success = true;
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using Location - '{location}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public override TransactionResult<UserInfo> Update(UserInfo appObj, int referenceKey)
        {
            return new TransactionResult<UserInfo>()
                {Success = false, Details = "BAD REQUEST - Operation not implemented"};
        }
    }

    public interface IUserInfoRepo
    {
        TransactionResult<int> Insert(UserInfo appObj);

        TransactionResult<UserInfo> Update(UserInfo appObj);

        TransactionResult<UserInfo> FindById(int id);

        TransactionResult<UserInfo> FindByUntappdId(int id);

        TransactionResult<ICollection<UserInfo>> FindByFirstName(string firstName);

        TransactionResult<ICollection<UserInfo>> FindByLastName(string lastName);

        TransactionResult<ICollection<UserInfo>> FindByLocation(string location);
    }
}