using ChatingApp.BackEnd.Entities;
using Microsoft.AspNetCore.Identity;

namespace ChatingApp.Models
{
    public class AppUser : IdentityUser
    {
        public required string DisplayName { get; set; }
        public string? ImageUrl { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        public Member Member { get; set; } = null!;
    }
}
