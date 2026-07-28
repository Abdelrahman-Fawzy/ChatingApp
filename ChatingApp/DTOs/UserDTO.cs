namespace ChatingApp.BackEnd.DTOs
{
    public class UserDTO
    {
        public Guid ID { get; set; }
        public required string DisplayName { get; set; }
        public required string Email { get; set; }
        public string? ImageURL { get; set; }
        public required string Token { get; set; }
    }
}
