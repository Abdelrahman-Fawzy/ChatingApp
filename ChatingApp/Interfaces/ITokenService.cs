using ChatingApp.Models;

namespace ChatingApp.BackEnd.Interfaces
{
    public interface ITokenService
    {
        public Task<string> CreateToken(AppUser user);
        public string CreateRefreshToken();
    }
}
