namespace ChatingApp.Models
{
    public class AppUser
    {
        public Guid ID { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
    }
}
