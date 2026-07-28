namespace ChatingApp.Models
{
    public class AppUser
    {
        public Guid ID { get; set; }
        public required string DisplayName { get; set; }
        public required string Email { get; set; }
        public required byte[] PasswordHash { get; set; }
        public required byte[] PasswordSalt { get; set; }
    }
}
