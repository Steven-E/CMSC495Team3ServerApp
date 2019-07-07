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
            const string sql = "INSERT INTO Brewery " +
                               "(BreweryName, Address, Phone, UntappdId, BreweryDbId, LabelUrl, OrgType) " +
                               "VALUES " +
                               "(@BreweryName, @Address, @Phone, @UntappdId, @BreweryDbId, @LabelUrl, @OrgType);" +
                               "SELECT LAST_INSERT_ID();";

            var retVal = new TransactionResult<Brewery>();
            retVal.Data = appObj;

            try
            {
                //int breweryId = -1;

                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data.BreweryId = connection.Query<int>(sql, new
                    {
                        BreweryName = appObj.BreweryName,
                        Address = appObj.Address,
                        Phone = appObj.Phone,
                        UntappdId = appObj.UntappdId,
                        BreweryDbId = appObj.BreweryDbId,
                        LabelUrl  = appObj.LabelUrl,
                        OrgType = appObj.OrgType
                    }).Single();
                }

                //if (retVal.Data.Beers.Count > 0)
                //{

                //}
                foreach (var beer in retVal.Data.Beers)
                {
                    if (beer.BeerId < 100)
                    {
                        beer.Brewery_FK = retVal.Data.BreweryId;

                        var beerRet = beerRepo.Insert(beer);

                        if (beerRet.Success)
                            beer.BeerId = beerRet.Data;
                    }
                }

                retVal.Data = appObj;
                retVal.Success = true;


            }
            catch (Exception e)
            {
                Log.Error($"Could not insert '{JsonConvert.SerializeObject(appObj)}.'", e);

                retVal.Details = e.Message;
                retVal.Success = false;
            }

            return retVal;
        }

        public TransactionResult<Brewery> AddOrUpdate(Brewery appObj)
        {
            if (appObj.BreweryId == 0)
            {
                return this.Insert(appObj);
            }

            return this.Update(appObj);
        }

        public override TransactionResult<Brewery> Update(Brewery appObj)
        {
            const string sql = "UPDATE Brewery SET " +
                               "BreweryName = @BreweryName " +
                               "Address =  @Address " +
                               "Phone =  @Phone " +
                               "UntappdId =  @UntappdId " +
                               "BreweryDbId =  @BreweryDbId " +
                               "LabelUrl =  @LabelUrl " +
                               "OrgType = @OrgType " +
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
                }

                foreach (var beer in retVal.Data.Beers)
                {
                    if (beer.BeerId < 100)
                    {
                        beer.Brewery_FK = retVal.Data.BreweryId;

                        var beerRet = beerRepo.Insert(beer);

                        if (beerRet.Success)
                            beer.BeerId = beerRet.Data;
                    }
                }

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
                }

                if (retVal?.Data != null && getBeers)
                    retVal.Data.Beers = beerRepo.FindByBreweryId(retVal.Data.BreweryId).Data.ToList();

                retVal.Success = true;
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using BreweryDbId - {id}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<ICollection<Brewery>> FindAllBreweries(bool getBeers)
        {
            const string sql = "SELECT * FROM Brewery;";

            var retVal = new TransactionResult<ICollection<Brewery>>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<Brewery>(sql).ToList();
                }

                if(getBeers)
                    foreach (var brewery in retVal.Data)
                    {
                        brewery.Beers = beerRepo.FindByBreweryId(brewery.BreweryId).Data.ToList();
                    }

                retVal.Success = true;
            }
            catch (Exception e)
            {
                Log.Error($"Ran into an error while trying to retrieve 'All' breweries and {(getBeers ? "associated " : "no ")}beers.", e);

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

        TransactionResult<Brewery> AddOrUpdate(Brewery appObj);

        //TransactionResult<Brewery> FindByBeerId(int beerId);

        TransactionResult<Brewery> FindById(int id, bool getBeers);

        TransactionResult<Brewery> FindByUntappdId(int id, bool getBeers);

        TransactionResult<Brewery> FindBreweryDbId(int id, bool getBeers);

        TransactionResult<ICollection<Brewery>> FindAllBreweries(bool getBeers);
    }
}