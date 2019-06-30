using System.Collections.Generic;
using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.Models.App
{
    public class UserInfo
    {
        public string UserName { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public string UserEmail { get; set; }

        public int UserId { get; set; }

        public int UntappdId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Location { get; set; }

        public List<UserBeerRanking> BeerRankings { get; set; }

        public List<SocialMediaAccount> SocialAccounts { get; set; }
    }
}