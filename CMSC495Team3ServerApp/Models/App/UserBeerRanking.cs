﻿using Newtonsoft.Json;

namespace CMSC495Team3ServerApp.Models.App
{
    public class UserBeerRanking
    {
        public float Score { get; set; }

        public int UserRankPosition { get; set; }

        public bool HaveTried { get; set; }

        public string UserReview { get; set; }

        public Beer Beer { get; set; }

        public int Beer_FK { get; set; }

        [JsonProperty("UserId")]
        public int User_FK { get; set; }
    }
}