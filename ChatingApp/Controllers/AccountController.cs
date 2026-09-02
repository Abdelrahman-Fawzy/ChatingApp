using ChatingApp.BackEnd.DTOs;
using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Extensions;
using ChatingApp.BackEnd.Interfaces;
using ChatingApp.Data;
using ChatingApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ChatingApp.BackEnd.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(AppDbContext context, ITokenService token)
        {
            _context = context;
            _tokenService = token;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register([FromBody] RegisterDTO registerDTO)
        {

            if (await EmailExists(registerDTO.Email)) {
                return BadRequest(new { Message = "Email Is Taken" });
            }

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                DisplayName = registerDTO.DisplayName,
                Email = registerDTO.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key,
                Member = new Member
                {
                    DisplayName = registerDTO.DisplayName,
                    Gender = registerDTO.Gender,
                    DateOfBirth = registerDTO.DateOfBirth,
                    City = registerDTO.City,
                    Country = registerDTO.Country
                }
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user.ToUserDTO(_tokenService);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Email == loginDTO.Email);

            if (user == null) 
            {
                return Unauthorized(new { Message = "Invalid User" });
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHashPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for(var i = 0; i < computedHashPassword.Length; i++)
            {
                if (computedHashPassword[i] != user.PasswordHash[i])
                {
                    return Unauthorized(new { Message = "Invalid Password" });
                }
            }

            return user.ToUserDTO(_tokenService);
        }

        private async Task<bool> EmailExists(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email == email);
        }
    }
}
