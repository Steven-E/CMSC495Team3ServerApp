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
    public class BeerRepo : RepoBase<Beer>, IBeerRepo
    {
        private readonly IBreweryRepo breweryRepo;

        public BeerRepo(ILogger logger, IConfigProvider configProvider) : base(logger, configProvider)
        {
        }

        public TransactionResult<int> Insert(Beer appObj)
        {
            const string sql = "INSERT INTO Beer (Brewery_FK, UntappdId, BreweryDbId, " +
                               "BeerName, ABV, Description, IBU, Style, ShortStyle, LabelUrl) " +
                               "VALUES " +
                               "(@BreweryId, @UntappdId, @BreweryDbId, @BeerName, " +
                               "@ABV, @Description, @IBU, @Style, @ShortStyle, @LabelUrl);" +
                               "SELECT LAST_INSERT_ID();";

            var retVal = new TransactionResult<int>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<int>(sql, new
                    {
                        BreweryId = appObj.Brewery_FK,
                        appObj.UntappdId,
                        appObj.BreweryDbId,
                        appObj.BeerName,
                        appObj.ABV,
                        appObj.Description,
                        appObj.IBU,
                        appObj.Style,
                        appObj.ShortStyle,
                        appObj.LabelUrl
                    }).Single();

                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not insert '{JsonConvert.SerializeObject(appObj)}.'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public override TransactionResult<Beer> Update(Beer appObj)
        {
            const string sql = "UPDATE Beer SET " +
                               "Brewery_FK = @BreweryId, " +
                               "UntappdId = @UntappdId, " +
                               "BreweryDbId = @BreweryDbId, " +
                               "BeerName = @BeerName, " +
                               "ABV = @ABV, " +
                               "Description = @Description, " +
                               "IBU = @IBU, " +
                               "Style = @Style, " +
                               "ShortStyle = @ShortStyle, " +
                               "LabelUrl = @LabelUrl " +
                               "WHERE BeerId = @BeerId;";

            var retVal = new TransactionResult<Beer>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    connection.Execute(sql, new
                    {
                        BreweryId = appObj.Brewery_FK,
                        appObj.UntappdId,
                        appObj.BreweryDbId,
                        appObj.BeerName,
                        appObj.ABV,
                        appObj.Description,
                        appObj.IBU,
                        appObj.Style,
                        appObj.ShortStyle,
                        appObj.LabelUrl,
                        appObj.BeerId
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

        public TransactionResult<Beer> FindUntappdId(int id)
        {
            var sql = "SELECT * FROM Beer WHERE " +
                      "UntappdId = @BeerId";

            var retVal = new TransactionResult<Beer>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<Beer>(sql, new
                    {
                        BeerId = id
                    }).FirstOrDefault();

                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error("Could not perform FIND using UntappdId - " +
                          $"'{id}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<Beer> FindById(int id)
        {
            const string sql = "SELECT * FROM Beer WHERE " +
                               "BeerId = @BeerId";

            var retVal = new TransactionResult<Beer>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<Beer>(sql, new
                    {
                        BeerId = id
                    }).FirstOrDefault();
                }

                //TODO: revisit this... I've created a circular D.I. dependency 
                //if (retVal.Data != null)
                //    retVal.Data.Brewery = breweryRepo.FindById(retVal.Data.Brewery_FK, false).Data;

                retVal.Success = true;
            }
            catch (Exception e)
            {
                Log.Error("Could not perform FIND using BeerId " +
                          $"'{id}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<ICollection<Beer>> FindByBreweryId(int breweryId)
        {
            const string sql = "SELECT * FROM  Beer WHERE Brewery_FK = @BreweryId";

            var retVal = new TransactionResult<ICollection<Beer>>();

            try
            {
                using (var connection = new MySqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<Beer>(sql, new
                    {
                        BreweryId = breweryId
                    }).ToList();

                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using Brewery_FK - '{breweryId}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public override TransactionResult<Beer> Update(Beer appObj, int referenceKey)
        {
            throw new NotImplementedException();
        }
    }

    public interface IBeerRepo
    {
        TransactionResult<int> Insert(Beer appObj);

        TransactionResult<Beer> Update(Beer appObj);

        TransactionResult<Beer> FindById(int id);

        TransactionResult<Beer> FindUntappdId(int id);

        TransactionResult<ICollection<Beer>> FindByBreweryId(int breweryId);
    }
}