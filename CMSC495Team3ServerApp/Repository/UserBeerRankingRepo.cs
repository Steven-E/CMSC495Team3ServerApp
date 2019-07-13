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
        private readonly IBeerRepo beerRepo;

        public UserBeerRankingRepo(ILogger logger, IConfigProvider configProvider, IBeerRepo beerRepo) : base(logger,
            configProvider)
        {
            this.beerRepo = beerRepo;
        }

        public TransactionResult<bool> Insert(UserBeerRanking appObj)
        {
            //TODO: Need to toy around with double quotes or single quotes in the sql query.
            const string sql =
                "INSERT INTO UserBeerRanking " +
                "(User_FK, Beer_FK, Score, UserRankPosition,  HaveTried, UserReview) " +
                "VALUES " +
                "(@User_FK, @Beer_FK, @Score, @UserRankPosition, @HaveTried, @UserReview);";
            //+ "SELECT LAST_INSERT_ID();";
            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    connection.Execute(sql,
                        new
                        {
                            appObj.User_FK,
                            Beer_FK = appObj.Beer.BeerId,
                            appObj.Score,
                            appObj.UserRankPosition,
                            appObj.HaveTried,
                            appObj.UserReview
                        });
                }

                return new TransactionResult<bool> {Data = true, Success = true};
            }
            catch (Exception e)
            {
                Log.Error($"Could not insert '{JsonConvert.SerializeObject(appObj)}.'", e);

                return new TransactionResult<bool> {Data = false, Success = false, Details = e.Message};
            }
        }

        public override TransactionResult<UserBeerRanking> Update(UserBeerRanking appObj)
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
                    connection.Execute(sql,
                        new
                        {
                            UserId = appObj.User_FK,
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

        public TransactionResult<UserBeerRanking> FindSingleByUserAndBeerId(int userId, int beerId)
        {
            const string sql = "SELECT * FROM UserBeerRanking " +
                               "WHERE User_FK = @UserId AND Beer_FK = @BeerId";

            var retVal = new TransactionResult<UserBeerRanking>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<UserBeerRanking, Beer, UserBeerRanking>(sql,
                        (r, b) =>
                        {
                            r.Beer = b;
                            return r;
                        },
                        splitOn: "Beer_FK",
                        param:
                        new
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

        public TransactionResult<ICollection<UserBeerRanking>> FindAllByUserId(int userId)
        {
            const string sql = "SELECT * FROM UserBeerRanking " +
                               "WHERE User_FK = @UserId";

            var retVal = new TransactionResult<ICollection<UserBeerRanking>>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<UserBeerRanking>(sql,
                        new
                        {
                            UserId = userId
                        }).ToList();
                }

                retVal.Success = true;
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using User_FK - '{userId}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<ICollection<UserBeerRanking>> FindAllByBeerId(int id)
        {
            const string sql = "SELECT * FROM UserBeerRanking " +
                               "WHERE Beer_FK = @id";

            var retVal = new TransactionResult<ICollection<UserBeerRanking>>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<UserBeerRanking>(sql,
                        new
                        {
                            id
                        }).ToList();
                }

                if (retVal.Data != null)
                    foreach (var beerRanking in retVal.Data)
                    {
                        if (beerRanking == null) continue;
                        beerRanking.Beer = beerRepo.FindById(beerRanking.Beer_FK).Data;
                    }

                retVal.Success = true;
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using Beer_FK - '{id}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<ICollection<UserBeerRanking>> FindAllByUntappdId(int id)
        {
            const string sql = "SELECT * FROM UserBeerRanking AS r " +
                               "INNER JOIN Beer AS b " +
                               "ON r.Beer_FK = b.BeerId " +
                               "WHERE b.UntappdId = @id";

            var retVal = new TransactionResult<ICollection<UserBeerRanking>>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    retVal.Data = connection.Query<UserBeerRanking, Beer, UserBeerRanking>(
                        sql,
                        (ranking, beer) =>
                        {
                            ranking.Beer = beer;
                            return ranking;
                        },
                        splitOn: "Beer_FK",
                        param:
                        new
                        {
                            id
                        }).Distinct().ToList();
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

        public TransactionResult<ICollection<UserBeerRanking>> FindAllByBreweryDbId(int id)
        {
            const string sql = "SELECT * FROM UserBeerRanking AS r " +
                               "INNER JOIN Beer AS b " +
                               "ON r.Beer_FK = b.BeerId " +
                               "WHERE b.BreweryDbId = @id";

            var retVal = new TransactionResult<ICollection<UserBeerRanking>>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    retVal.Data = connection.Query<UserBeerRanking, Beer, UserBeerRanking>(
                        sql,
                        (ranking, beer) =>
                        {
                            ranking.Beer = beer;
                            return ranking;
                        },
                        splitOn: "Beer_FK",
                        param:
                        new
                        {
                            id
                        }).Distinct().ToList();
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

        public override TransactionResult<UserBeerRanking> Update(UserBeerRanking appObj, int referenceKey)
        {
            throw new NotImplementedException();
        }
    }

    public interface IUserBeerRankingRepo
    {
        TransactionResult<bool> Insert(UserBeerRanking appObj);

        TransactionResult<UserBeerRanking> Update(UserBeerRanking appObj);

        TransactionResult<UserBeerRanking> FindSingleByUserAndBeerId(int userId, int beerId);

        TransactionResult<ICollection<UserBeerRanking>> FindAllByUserId(int userId);

        TransactionResult<ICollection<UserBeerRanking>> FindAllByBeerId(int id);

        TransactionResult<ICollection<UserBeerRanking>> FindAllByUntappdId(int id);

        TransactionResult<ICollection<UserBeerRanking>> FindAllByBreweryDbId(int id);
    }
}