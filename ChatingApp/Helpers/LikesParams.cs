namespace ChatingApp.BackEnd.Helpers
{
    public class LikesParams : PagingParams
    {
        public string MemberID { get; set; } = "";
        public string Predicate { get; set; } = "liked";
    }
}
