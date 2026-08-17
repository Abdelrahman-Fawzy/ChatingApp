using ChatingApp.BackEnd.DTOs;
using ChatingApp.BackEnd.Interfaces;
using ChatingApp.Models;

namespace ChatingApp.BackEnd.Extensions
{
    public static class AppUserExtension
    {
        public static UserDTO ToUserDTO(this AppUser user, ITokenService _tokenService)
        {
            var userDto = new UserDTO
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            };

            return userDto;
        }
    }
}
