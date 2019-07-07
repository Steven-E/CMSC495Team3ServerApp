//using Newtonsoft.Json;

//namespace CMSC495Team3ServerApp.Models.Untappd
//{
//    public class UntappdAuthResponse
//    {
//        [JsonProperty("meta")]
//        public UntappdAuthMeta Meta { get; set; }

//        [JsonProperty("response")]
//        public UntappdAuthContent Response { get; set; }
//    }

//    public class UntappdAuthMeta
//    {
//        [JsonProperty("http_code")]
//        public int HttpCode { get; set; }
//    }

//    public class UntappdAuthContent
//    {
//        [JsonProperty("access_token")]
//        public string AccessToken{ get; set; } 
//    }
//}

using System;
using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.Models.Untappd
{
    //public class UntappdAuthResponse
    //{
    //    //[JsonProperty("meta")]
    //    public UntappdAuthMeta meta { get; set; }

    //    //[JsonProperty("response")]
    //    public UntappdAuthContent response { get; set; }
    //}

    //public class UntappdAuthMeta
    //{
    //    //[JsonProperty("http_code")]
    //    public int http_code { get; set; }
    //}

    //public class UntappdAuthContent
    //{
    //    //[JsonProperty("access_token")]
    //    public string access_token { get; set; }
    //}

    public class UntappdResponse
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("response")]
        //public Response Response { get; set; }
        public dynamic Response { get; set; }
    }

    public class Meta
    {
        [JsonProperty("code")]
        public int Code { get; set; }
    }

    public class BeerSearchResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("brewery_id")]
        public int BreweryId { get; set; }

        [JsonProperty("search_type")]
        public string SearchType { get; set; }

        [JsonProperty("found")]
        public long Found { get; set; }

        [JsonProperty("offset")]
        public long Offset { get; set; }

        [JsonProperty("limit")]
        public long Limit { get; set; }

        [JsonProperty("term")]
        public string Term { get; set; }

        [JsonProperty("parsed_term")]
        public string ParsedTerm { get; set; }

        [JsonProperty("beers")]
        public Beers Beers { get; set; }
    }



    public class Beers
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("items")]
        public Item[] Items { get; set; }
    }

    public class Item
    {
        [JsonProperty("beer")]
        public Beer Beer { get; set; }

        [JsonProperty("brewery")]
        public Brewery Brewery { get; set; }
    }

    //public partial class Beer
    //{
    //    [JsonProperty("bid")]
    //    public long Bid { get; set; }

    //    [JsonProperty("beer_name")]
    //    public string BeerName { get; set; }

    //    [JsonProperty("beer_label")]
    //    public Uri BeerLabel { get; set; }

    //    [JsonProperty("beer_abv")]
    //    public double BeerAbv { get; set; }

    //    [JsonProperty("beer_slug")]
    //    public string BeerSlug { get; set; }

    //    [JsonProperty("beer_ibu")]
    //    public long BeerIbu { get; set; }

    //    [JsonProperty("beer_description")]
    //    public string BeerDescription { get; set; }

    //    [JsonProperty("created_at")]
    //    public string CreatedAt { get; set; }

    //    [JsonProperty("beer_style")]
    //    public string BeerStyle { get; set; }

    //    [JsonProperty("in_production")]
    //    public long InProduction { get; set; }

    //    [JsonProperty("auth_rating")]
    //    public long AuthRating { get; set; }

    //    [JsonProperty("wish_list")]
    //    public bool WishList { get; set; }
    //}

}