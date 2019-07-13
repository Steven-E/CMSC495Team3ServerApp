using System.Collections.Generic;
using CMSC495Team3ServerApp.Models.App;
using CMSC495Team3ServerApp.Models.Untappd;
using Beer = CMSC495Team3ServerApp.Models.App.Beer;
using Brewery = CMSC495Team3ServerApp.Models.App.Brewery;

namespace CMSC495Team3ServerApp.Adapters
{
    public class UntappdToAppAdapter
    {
        public static UserInfo Get(User uObj)
        {
            var retVal = new UserInfo();
            retVal.FirstName = uObj.first_name;
            retVal.LastName = uObj.last_name;
            retVal.Location = uObj.location;
            retVal.UntappdId = uObj.uid;
            retVal.UserName = uObj.user_name;

            //TODO: get UserBeerRankings.
            retVal.BeerRankings = new List<UserBeerRanking>();

            foreach (var checkinsItem in uObj.Checkins.Items)
            {
                var review = Get(checkinsItem);

                retVal.BeerRankings.Add(review);
            }

            return retVal;
        }

        public static Beer Get(Models.Untappd.Beer uObj)
        {
            var retVal = new Beer();
            retVal.Description = uObj.beer_description;
            retVal.UntappdId = uObj.bid;
            retVal.LabelUrl = uObj.beer_label;
            retVal.IBU = uObj.beer_ibu;
            retVal.BeerName = uObj.beer_name;
            retVal.Style = uObj.beer_style;
            retVal.ABV = uObj.beer_abv;

            //TODO: fix this

            return retVal;
        }

        public static UserBeerRanking Get(CheckinsItem uObj)
        {
            var retVal = new UserBeerRanking();
            retVal.HaveTried = true;
            retVal.Score = uObj.RatingScore;
            retVal.UserReview = uObj.CheckinComment;
            retVal.Beer = Get(uObj.Beer);

            return retVal;
        }

        public static Brewery Get(Models.Untappd.Brewery uObj)
        {
            var retVal = new Brewery();
            retVal.BreweryName = uObj.brewery_name;
            retVal.LabelUrl = uObj.brewery_label;
            retVal.OrgType = uObj.brewery_type;
            retVal.UntappdId = uObj.brewery_id;
            retVal.Beers = new List<Beer>();
            retVal.Address = $"{uObj.location.brewery_city}, {uObj.location.brewery_state}";
            return retVal;
        }
    }
}