using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CMSC495Team3ServerApp.Logging;
using CMSC495Team3ServerApp.Models.App;
using CMSC495Team3ServerApp.Provider;
using Dapper;
using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.Repository
{
    public class BeerRepo : RepoBase<Beer>, IBeerRepo
    {
        public BeerRepo(ILogger logger, IConfigProvider configProvider) : base(logger, configProvider)
        {
        }

        public TransactionResult<int> Insert(Beer appObj)
        {
            const string sql = "INSERT INTO Beer (Brewery_FK, UntappdId, BreweryDbId, " +
                               "BeerName, ABV, Description, IBU, Style, ShortStyle, LabelUrl) " +
                               "VALUES " +
                               "(@BreweryId, @UntappdId, @BreweryDbId, '@BeerName', " +
                               "@ABV, '@Description', @IBU, '@Style', '@ShortStyle', '@LabelUrl;')" +
                               "SELECT LAST_INSERT_ID();";

            var retVal = new TransactionResult<int>();

            try
            {
                using (var connection = new SqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<int>(sql, new
                    {
                        BreweryId = appObj.Brewery.BreweryId,
                        UntappdId = appObj.UntappdId,
                        BreweryDbId = appObj.BreweryDbId,
                        BeerName = appObj.BeerName,
                        ABV = appObj.ABV,
                        Description = appObj.Description,
                        IBU = appObj.IBU,
                        Style = appObj.Style,
                        ShortStyle = appObj.ShortStyle,
                        LabelUrl = appObj.LabelUrl
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
                               "BrewerId = @BreweryId " +
                               "UntappdId = @UntappdId " +
                               "BreweryDbId = @BreweryDbId " +
                               "BeerName = '@BeerName' " +
                               "ABV = @ABV " +
                               "Description = '@Description' " +
                               "IBU = @IBU " +
                               "Style = '@Style' " +
                               "ShortStyle = '@ShortStyle' " +
                               "LabelUrl = '@LabelUrl' " +
                               "WHERE BeerId = '@BeerId';";

            var retVal = new TransactionResult<Beer>();

            try
            {
                using (var connection = new SqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    connection.Execute(sql, new
                    {
                        BreweryId = appObj.Brewery.BreweryId,
                        UntappdId = appObj.UntappdId,
                        BreweryDbId = appObj.BreweryDbId,
                        BeerName = appObj.BeerName,
                        ABV = appObj.ABV,
                        Description = appObj.Description,
                        IBU = appObj.IBU,
                        Style = appObj.Style,
                        ShortStyle = appObj.ShortStyle,
                        LabelUrl = appObj.LabelUrl
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

        public TransactionResult<Beer> Find(int beerId, bool isUntappd)
        {
            string sql = "SELECT * FROM Beer WHERE " +
                         $"{(isUntappd ? "UntappdId = " : "BeerId = ")}" +
                         $"@BeerId";

            var retVal = new TransactionResult<Beer>();

            try
            {
                using (var connection = new SqlConnection(Config.DatabaseConnectionString))
                {
                    connection.Open();
                    retVal.Data = connection.Query<Beer>(sql, new
                    {
                        BeerId = beerId
                    }).FirstOrDefault();

                    retVal.Success = true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not perform FIND using {(isUntappd ? "UntappdId" : "BeerId")} - " +
                          $"'{beerId}'", e);

                retVal.Success = false;
                retVal.Details = e.Message;
            }

            return retVal;
        }

        public TransactionResult<ICollection<Beer>> Find(int breweryId)
        {
            const string sql = "SELECT * FROM  Beer WHERE Brewery_FK = @BreweryId";

            var retVal = new TransactionResult<ICollection<Beer>>();

            try
            {
                using (var connection = new SqlConnection(Config.DatabaseConnectionString))
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
            throw new System.NotImplementedException();
        }
    }

    public interface IBeerRepo
    {
        TransactionResult<int> Insert(Beer appObj);
        
        TransactionResult<Beer> Update(Beer appObj);
        //TransactionResult<Beer> Update(Beer appObj, int );

        TransactionResult<Beer> Find(int beerId, bool isUntappd);

        TransactionResult<ICollection<Beer>> Find(int breweryId);
    }
}