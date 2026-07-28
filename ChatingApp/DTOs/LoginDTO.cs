using System.ComponentModel.DataAnnotations;

namespace ChatingApp.BackEnd.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
}
