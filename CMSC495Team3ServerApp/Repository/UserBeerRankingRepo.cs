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
    public class UserBeerRankingRepo : RepoBase<UserBeerRanking>, IUserBeerRankingRepo
    {
        public UserBeerRankingRepo(ILogger logger, IConfigProvider configProvider) : base(logger, configProvider)
        {
        }

        public void Insert(UserBeerRanking appObj, int userId)
        {
            //TODO: Need to toy around with double quotes or single quotes in the sql query.
            const string sql =
                "INSERT INTO UserBeerRanking " +
                "(User_FK, " +
                "Beer_FK, " +
                "Score, " +
                "UserRankPosition, " +
                " HaveTried, " +
                "UserReview) " +
                "VALUES " +
                "(@User_FK, " +
                "@Beer_FK, " +
                "@Score, " +
                "@UserRankPosition, " +
                "@HaveTried, " +
                "@UserReview);"; //+
            //"SELECT LAST_INSERT_ID();";
            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    connection.Execute(sql, new
                    {
                        User_FK = userId,
                        Beer_FK = appObj.Beer.BeerId,
                        appObj.Score,
                        appObj.UserRankPosition,
                        appObj.HaveTried,
                        appObj.UserReview
                    });
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not insert '{JsonConvert.SerializeObject(appObj)}.'", e);
            }
        }

        public override TransactionResult<UserBeerRanking> Update(UserBeerRanking appObj, int userId)
        {
            const string sql =
                "UPDATE UserBeerRanking SET " +
                "Score = @Score " +
                "UserRankPosition = @UserRankPosition " +
                "HaveTried = @HaveTried " +
                "UserReview = @UserReview " +
                "WHERE User_FK = @UserId " +
                "AND Beer_FK = @BeerId";

            var retVal = new TransactionResult<UserBeerRanking>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    connection.Execute(sql, new
                    {
                        UserId = userId,
                        appObj.Beer.BeerId,
                        appObj.Score,
                        appObj.UserRankPosition,
                        appObj.HaveTried,
                        appObj.UserReview
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

        public TransactionResult<UserBeerRanking> Find(int userId, int beerId)
        {
            const string sql = "SELECT * FROM UserBeerRankings" +
                               "WHERE User_FK = @UserId AND Beer_FK = @BeerId";

            var retVal = new TransactionResult<UserBeerRanking>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<UserBeerRanking>(sql, new
                    {
                        UserId = userId,
                        BeerId = beerId
                    }).FirstOrDefault();

                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using User_FK - '{userId}', Beer_FK - '{beerId}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<ICollection<UserBeerRanking>> Find(int userId)
        {
            const string sql = "SELECT * FROM UserBeerRankings" +
                               "WHERE User_FK = @UserId";

            var retVal = new TransactionResult<ICollection<UserBeerRanking>>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<UserBeerRanking>(sql, new
                    {
                        UserId = userId
                    }).ToList();

                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using User_FK - '{userId}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<ICollection<UserBeerRanking>> Find(int beerId, bool isUntappdId)
        {
            //TODO: figure this part out... 
            var sql = "SELECT * FROM UserBeerRankings u" +
                      (isUntappdId
                          ? "INNER JOIN Beer b ON u.Beer_FK = b.UntappId " +
                            "WHERE b.UntappdId = @beerId"
                          : "WHERE BeerId = @beerId"
                      );
            //$"WHERE { (!isUntappdId ? "Beer_FK = @BeerId" : "" )}";

            var retVal = new TransactionResult<ICollection<UserBeerRanking>>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<UserBeerRanking>(sql, new
                    {
                        BeerId = beerId
                    }).ToList();

                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using {(isUntappdId ? "UntappdId" : "BeerId")} - " +
                          $"'{beerId}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public override TransactionResult<UserBeerRanking> Update(UserBeerRanking appObj)
        {
            throw new NotImplementedException();
        }
    }

    public interface IUserBeerRankingRepo
    {
        void Insert(UserBeerRanking appObj, int userId);

        TransactionResult<UserBeerRanking> Update(UserBeerRanking appObj, int userId);

        TransactionResult<UserBeerRanking> Find(int userId, int beerId);

        TransactionResult<ICollection<UserBeerRanking>> Find(int userId);

        TransactionResult<ICollection<UserBeerRanking>> Find(int beerId, bool isUntappdId);
    }
}