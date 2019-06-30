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

        public void Insert(UserInfo appObj)
        {
            const string sql =
                "INSERT INTO UserInfo (UserName, Password, UserEmail, " +
                " FirstName, LastName, Location, UntappdId) VALUES " +
                "(\"@UserName\", \"@Password\", \"@UserEmail\", \"@FirstName\", \"@LastName\", \"@Location\", \"@UntappdId\");" +
                "SELECT LAST_INSERT_ID();";
            try
            {
                int userId;

                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    userId = connection.Query<int>(sql, new
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

                foreach (var ranking in appObj.BeerRankings) rankRepo.Insert(ranking, userId);

                foreach (var account in appObj.SocialAccounts) socialAccRepo.Insert(account, userId);
            }
            catch (Exception e)
            {
                Log.Error($"Could not insert '{JsonConvert.SerializeObject(appObj)}.'", e);
            }
        }

        public override TransactionResult<UserInfo> Update(UserInfo appObj)
        {
            const string sql = "UPDATE UserInfo SET " +
                               "UserName = '@UserName' " +
                               "UserEmail = '@UserEmail' " +
                               "FirstName = '@FirstName' " +
                               "LastName = '@LastName' " +
                               "Location = '@Location' " +
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

                foreach (var ranking in appObj.BeerRankings)
                    userRankingCollection.Add(rankRepo.Update(ranking, appObj.UserId));

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

        public TransactionResult<UserInfo> Find(int userId, bool isUntappdId)
        {
            var sql = "SELECT * FROM UserInfo WHERE " +
                      $"{(isUntappdId ? "UntappdId = " : "UserId = ")}" +
                      "@UserId";

            var retVal = new TransactionResult<UserInfo>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<UserInfo>(sql, new
                    {
                        UserId = userId
                    }).FirstOrDefault();

                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using {(isUntappdId ? "UntappdId" : "UserId")} - " +
                          $"'{userId}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public override TransactionResult<UserInfo> Update(UserInfo appObj, int referenceKey)
        {
            throw new NotImplementedException();
        }
    }

    public interface IUserInfoRepo
    {
        void Insert(UserInfo appObj);

        TransactionResult<UserInfo> Update(UserInfo appObj);

        TransactionResult<UserInfo> Find(int userId, bool isUntappdId);

        //TransactionResult<UserInfo> Find(int untappdId);
    }
}