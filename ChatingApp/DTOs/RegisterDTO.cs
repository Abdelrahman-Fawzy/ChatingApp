using System.ComponentModel.DataAnnotations;

namespace ChatingApp.BackEnd.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string DisplayName { get; set; } = String.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = String.Empty;
        [Required]
        [MinLength(4)]
        public string Password { get; set; } = String.Empty;
        [Required]
        public string Gender { get; set; } = String.Empty;
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        public string City { get; set; } = String.Empty;
        [Required]
        public string Country { get; set; } = String.Empty;
    }
}
