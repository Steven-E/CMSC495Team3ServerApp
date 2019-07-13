using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.Models.App
{
    public class Beer
    {
        //public Brewery Brewery { get; set; }

        public int BeerId { get; set; }

        public int UntappdId { get; set; }

        public int BreweryDbId { get; set; }

        public string BeerName { get; set; }

        public float ABV { get; set; }

        public string Description { get; set; }

        public int IBU { get; set; }

        public string Style { get; set; }

        public string ShortStyle { get; set; }

        //TODO: consider changing this to a locally cached image path
        public string LabelUrl { get; set; }

        [JsonProperty("BreweryId")]
        public int Brewery_FK { get; set; }

        //[JsonIgnore]
        //public int Brewery_FK { get; set; }
    }
}