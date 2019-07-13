using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.Models.Untappd
{
    public class UntappdResponse
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("response")]
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
}