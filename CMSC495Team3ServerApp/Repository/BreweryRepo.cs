using System;
using System.Data.SqlClient;
using System.Linq;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Models.App;
using CMSC495Team3ServerApp.Provider;
using Dapper;
using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.Repository
{
    public class BreweryRepo : RepoBase<Brewery>, IBreweryRepo
    {
        private readonly IBeerRepo beerRepo;

        public BreweryRepo(ILogger logger, IConfigProvider configProvider, IBeerRepo beerRepo) : base(logger, configProvider)
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
                using (var connection = new SqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    connection.Execute(sql, new
                    {
                        BreweryName = appObj.BreweryName,
                        Address = appObj.Address,
                        Phone = appObj.Phone,
                        UntappdId = appObj.UntappdId,
                        BreweryDbId = appObj.BreweryDbId,
                        LabelUrl = appObj.LabelUrl,
                        OrgType = appObj.OrgType
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
                using (var connection = new SqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    connection.Execute(sql, new
                    {
                        BreweryName = appObj.BreweryName,
                        Address = appObj.Address,
                        Phone = appObj.Phone,
                        UntappdId = appObj.UntappdId,
                        BreweryDbId = appObj.BreweryDbId,
                        LabelUrl = appObj.LabelUrl,
                        OrgType = appObj.OrgType,
                        BreweryId = appObj.BreweryId
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

        public TransactionResult<Brewery> Find(int breweryId, bool isUntappd)
        {
            string sql = "SELECT * FROM Brewery WHERE " +
                         $"{(isUntappd ? "UntappdId = " : "BreweryId = ")} @BreweryId";

            var retVal = new TransactionResult<Brewery>();

            try
            {
                using (var connection = new SqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<Brewery>(sql, new
                    {
                        BrewerId = breweryId
                    }).FirstOrDefault();

                    retVal.Data.Beers = beerRepo.Find(breweryId).Data.ToList();
                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using {(isUntappd ? "UntappdId" : "BreweryId")} - " +
                          $"'{breweryId}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public override TransactionResult<Brewery> Update(Brewery appObj, int referenceKey)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IBreweryRepo
    {
        TransactionResult<Brewery> Insert(Brewery appObj);

        TransactionResult<Brewery> Update(Brewery appObj);

        TransactionResult<Brewery> Find(int breweryId, bool isUntappd);
    }
}