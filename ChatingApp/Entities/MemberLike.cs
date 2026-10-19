namespace ChatingApp.BackEnd.Entities
{
    public class MemberLike
    {
        public required string SourceMemberID { get; set; }
        public Member SourceMember { get; set; } = null!;
        public required string TargetMemberID { get; set; }
        public Member TargetMember { get; set; } = null!;
    }
}
