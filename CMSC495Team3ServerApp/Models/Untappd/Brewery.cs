namespace CMSC495Team3ServerApp.Models.Untappd
{
    public class Brewery
    {
        public int brewery_id { get; set; }

        public string brewery_name { get; set; }

        public string brewery_slug { get; set; }

        public string brewery_label { get; set; }

        public string country_name { get; set; }

        public int brewery_in_production { get; set; }

        public int is_independent { get; set; }

        public int beer_count { get; set; }

        public Contact contact { get; set; }

        public string brewery_type { get; set; }

        public int brewery_type_id { get; set; }

        public Location location { get; set; }

        public string brewery_description { get; set; }
    }
}