using ChatingApp.Models;

namespace ChatingApp.BackEnd.Interfaces
{
    public interface ITokenService
    {
        public string CreateToken(AppUser user);
    }
}
