using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.Models.Untappd
{
    public class Checkins
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("items")]
        public CheckinsItem[] Items { get; set; }
    }

    public class CheckinsItem
    {
        [JsonProperty("checkin_id")]
        public long CheckinId { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("checkin_comment")]
        public string CheckinComment { get; set; }

        [JsonProperty("rating_score")]
        public float RatingScore { get; set; }

        [JsonProperty("beer")]
        public Beer Beer { get; set; }

        [JsonProperty("brewery")]
        public Brewery Brewery { get; set; }
    }
}