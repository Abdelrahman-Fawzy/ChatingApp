namespace ChatingApp.BackEnd.DTOs
{
    public class PhotoForApproval
    {
        public int Id { get; set; }
        public required string Url { get; set; }
        public required string MemberId { get; set; } = null!;
        public bool IsApproved { get; set; }
    }
}
