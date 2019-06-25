namespace CMSC495Team3ServerApp.Models.Untappd
{
    public class User
    {
        public int uid { get; set; }
        public int id { get; set; }
        public string user_name { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string user_avatar { get; set; }
        public string user_avatar_hd { get; set; }
        public string user_cover_photo { get; set; }
        public int user_cover_photo_offset { get; set; }
        public int is_private { get; set; }
        public string location { get; set; }
        public string url { get; set; }
        public string bio { get; set; }
        public int is_supporter { get; set; }
        public string relationship { get; set; }
        public string untappd_url { get; set; }
        public string account_type { get; set; }
        //public Stats stats { get; set; }
        public string date_joined { get; set; }
    }
}