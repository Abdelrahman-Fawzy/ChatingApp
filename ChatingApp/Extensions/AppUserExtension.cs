using ChatingApp.BackEnd.DTOs;
using ChatingApp.BackEnd.Interfaces;
using ChatingApp.Models;
using Microsoft.AspNetCore.Identity;

namespace ChatingApp.BackEnd.Extensions
{
    public static class AppUserExtension
    {
        public static async Task<UserDTO> ToUserDTO(this AppUser user, ITokenService _tokenService, UserManager<AppUser> _userManager)
        {
            var userDto = new UserDTO
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email!,
                ImageURL = user.ImageUrl,
                Token = await _tokenService.CreateToken(user),
                Roles = await _userManager.GetRolesAsync(user)
            };

            return userDto;
        }
    }
}
