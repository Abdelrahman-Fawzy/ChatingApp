using ChatingApp.BackEnd.DTOs;
using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Extensions;
using ChatingApp.BackEnd.Interfaces;
using ChatingApp.Data;
using ChatingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ChatingApp.BackEnd.Controllers
{
    public class AccountController : BaseApiController
    {
        //private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(UserManager<AppUser> userManager, ITokenService token)
        {
            _userManager = userManager;
            _tokenService = token;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register([FromBody] RegisterDTO registerDTO)
        {
            var user = new AppUser
            {
                DisplayName = registerDTO.DisplayName,
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                Member = new Member
                {
                    DisplayName = registerDTO.DisplayName,
                    Gender = registerDTO.Gender,
                    DateOfBirth = registerDTO.DateOfBirth,
                    City = registerDTO.City,
                    Country = registerDTO.Country
                }
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if(!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("identity", error.Description);
                }

                return ValidationProblem();
            }

            await _userManager.AddToRoleAsync(user, "Member");

            await SetRefreshToken(user);

            return await user.ToUserDTO(_tokenService, _userManager);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null) return Unauthorized(new { Message = "Invalid User" });

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if(!result) return Unauthorized(new { Message = "Invalid Password" });

            await SetRefreshToken(user);

            return await user.ToUserDTO(_tokenService, _userManager);
        }

        [HttpPost("Logout")]
        public async Task<ActionResult> Logout()
        {

            var token = Request.Cookies["refreshToken"];
            if(!string.IsNullOrEmpty(token))
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == token);
                if(user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiry = null;

                    await _userManager.UpdateAsync(user);
                }
            }


            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok();
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<UserDTO>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken)) return Unauthorized(new { Message = "Invalid Refresh Token" });

            var user = await _userManager.Users.FirstOrDefaultAsync(
                u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry > DateTime.UtcNow
            );

            if (user == null) return Unauthorized(new { Message = "Invalid Refresh Token" });

            await SetRefreshToken(user);

            return await user.ToUserDTO(_tokenService, _userManager);
        }

        private async Task SetRefreshToken(AppUser user)
        {
            var refreshaToken = _tokenService.CreateRefreshToken();
            user.RefreshToken = refreshaToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshaToken, cookieOption);
        }
    }
}
