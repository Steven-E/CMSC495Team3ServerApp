namespace CMSC495Team3ServerApp.Models.App
{
    public class UserBeerRanking
    {
        public float Score { get; set; }

        public int UserRankPosition { get; set; }

        public bool HaveTried { get; set; }

        //public int UntappdReviewId { get; set; }

        //public int BreweryDbId { get; set; }

        //public string Description { get; set; }

        public string UserReview { get; set; }

        public Beer Beer { get; set; }
    }
}