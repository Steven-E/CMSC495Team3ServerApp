namespace CMSC495Team3ServerApp.Models.App
{
    public class Beer
    {
        public int BreweryId { get; set; }

        public int BeerId { get; set; }

        public int UntappdId { get; set; }

        public int BrewerDbId { get; set; }

        public string BeerName { get; set; }

        public float ABV { get; set; }

        public string Description { get; set; }

        public int IBU { get; set; }

        public string Style { get; set; }

        public string ShortStyle { get; set; }

        //TODO: consider changing this to a locally cached image path
        public string LabelUrl { get; set; }
    }
}