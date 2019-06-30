using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.Models.App
{
    public class SocialMediaAccount
    {
        public string AccountId { get; set; }

        public SocialMedia Vendor { get; set; }

        [JsonIgnore]
        public string VendorName { get; set; }
    }
}