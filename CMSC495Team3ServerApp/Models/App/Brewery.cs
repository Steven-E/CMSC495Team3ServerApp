using System.Collections.Generic;

namespace CMSC495Team3ServerApp.Models.App
{
    public class Brewery
    {
        public string BreweryName { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public int BreweryId { get; set; }

        public int UntappdId { get; set; }

        public int BreweryDbId { get; set; }

        //TODO: consider changing this to a locally cached image path
        public string LabelUrl { get; set; }

        public string OrgType { get; set; }

        public List<Beer> Beers { get; set; }
    }
}