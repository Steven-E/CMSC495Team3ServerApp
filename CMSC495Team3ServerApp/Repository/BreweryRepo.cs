using System;
using System.Linq;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Models.App;
using CMSC495Team3ServerApp.Provider;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.Repository
{
    public class BreweryRepo : RepoBase<Brewery>, IBreweryRepo
    {
        private readonly IBeerRepo beerRepo;

        public BreweryRepo(ILogger logger, IConfigProvider configProvider, IBeerRepo beerRepo) : base(logger,
            configProvider)
        {
            this.beerRepo = beerRepo;
        }

        public TransactionResult<Brewery> Insert(Brewery appObj)
        {
            const string sql = "INSERT INTO Brewery + " +
                               "(BreweryName, " +
                               "Address, " +
                               "Phone, " +
                               "UntappdId, " +
                               "BreweryDbId, " +
                               "LabelUrl, " +
                               "OrgType) " +
                               "VALUES " +
                               "('@BreweryName', " +
                               "'@Address', " +
                               "'@Phone', " +
                               "UntappdId, " +
                               "BreweryDbId, " +
                               "'@LabelUrl', " +
                               "'@OrgType');";

            var retVal = new TransactionResult<Brewery>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    connection.Execute(sql, new
                    {
                        appObj.BreweryName,
                        appObj.Address,
                        appObj.Phone,
                        appObj.UntappdId,
                        appObj.BreweryDbId,
                        appObj.LabelUrl,
                        appObj.OrgType
                    });

                    retVal.Data = appObj;
                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not insert '{JsonConvert.SerializeObject(appObj)}.'", e);

                retVal.Details = e.Message;
                retVal.Success = false;
            }

            return retVal;
        }

        public override TransactionResult<Brewery> Update(Brewery appObj)
        {
            const string sql = "UPDATE Brewery SET " +
                               "BreweryName = '@BreweryName' " +
                               "Address =  '@Address' " +
                               "Phone =  '@Phone' " +
                               "UntappdId =  @UntappdId " +
                               "BreweryDbId =  @BreweryDbId " +
                               "LabelUrl =  '@LabelUrl' " +
                               "OrgType = '@OrgType' " +
                               "WHERE BreweryId = @BreweryId";

            var retVal = new TransactionResult<Brewery>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    connection.Execute(sql, new
                    {
                        appObj.BreweryName,
                        appObj.Address,
                        appObj.Phone,
                        appObj.UntappdId,
                        appObj.BreweryDbId,
                        appObj.LabelUrl,
                        appObj.OrgType,
                        appObj.BreweryId
                    });

                    retVal.Data = appObj;
                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not update '{JsonConvert.SerializeObject(appObj)}.'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        //public TransactionResult<Brewery> FindByBeerId(int beerId)
        //{
        //    const string sql = "SELECT * FROM Brewery WHERE " +
        //                       "BreweryId = @Id";

        //    var retVal = new TransactionResult<Brewery>();

        //    try
        //    {

        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error($"Could not perform FIND using BreweryId - {id}'", e);

        //        retVal.Success = false;
        //        retVal.Details = e.Message;
        //    }

        //    return retVal;
        //}

        public TransactionResult<Brewery> FindById(int id, bool getBeers)
        {
            const string sql = "SELECT * FROM Brewery WHERE " +
                               "BreweryId = @Id";

            var retVal = new TransactionResult<Brewery>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<Brewery>(sql, new
                    {
                        Id = id
                    }).FirstOrDefault();

                    if (retVal?.Data != null && getBeers)
                        retVal.Data.Beers = beerRepo.FindByBreweryId(id).Data.ToList();

                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using BreweryId - {id}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<Brewery> FindByUntappdId(int id, bool getBeers)
        {
            const string sql = "SELECT * FROM Brewery WHERE " +
                               "UntappdId = @Id";

            var retVal = new TransactionResult<Brewery>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<Brewery>(sql, new
                    {
                        Id = id
                    }).FirstOrDefault();

                    if (retVal?.Data != null && getBeers)
                        retVal.Data.Beers = beerRepo.FindByBreweryId(retVal.Data.BreweryId).Data.ToList();

                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using UntappdId - {id}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<Brewery> FindBreweryDbId(int id, bool getBeers)
        {
            const string sql = "SELECT * FROM Brewery WHERE " +
                               "BreweryDbId = @Id";

            var retVal = new TransactionResult<Brewery>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<Brewery>(sql, new
                    {
                        Id = id
                    }).FirstOrDefault();

                    if (retVal?.Data != null && getBeers)
                        retVal.Data.Beers = beerRepo.FindByBreweryId(retVal.Data.BreweryId).Data.ToList();

                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using BreweryDbId - {id}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public override TransactionResult<Brewery> Update(Brewery appObj, int referenceKey)
        {
            throw new NotImplementedException();
        }
    }

    public interface IBreweryRepo
    {
        TransactionResult<Brewery> Insert(Brewery appObj);

        TransactionResult<Brewery> Update(Brewery appObj);

        //TransactionResult<Brewery> FindByBeerId(int beerId);

        TransactionResult<Brewery> FindById(int id, bool getBeers);

        TransactionResult<Brewery> FindByUntappdId(int id, bool getBeers);

        TransactionResult<Brewery> FindBreweryDbId(int id, bool getBeers);
    }
}