using ChatingApp.BackEnd.DTOs;
using ChatingApp.BackEnd.Entities;
using ChatingApp.Data;
using ChatingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ChatingApp.BackEnd.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            var members = JsonSerializer.Deserialize<List<SeedUserDTO>>(memberData);

            if(members == null)
            {
                Console.WriteLine("No members Was Found");
                return;
            }

            foreach (var member in members)
            {
                using var hmac = new HMACSHA512();

                var user = new AppUser
                {
                    Id = member.Id,
                    Email = member.Email,
                    UserName = member.Email,
                    DisplayName = member.DisplayName,
                    ImageUrl = member.ImageUrl,
                    Member = new Member
                    {
                        Id = member.Id,
                        DisplayName = member.DisplayName,
                        City = member.City,
                        Country = member.Country,
                        Gender = member.Gender,
                        DateOfBirth = member.DateOfBirth,
                        Created = member.Created,
                        Description = member.Description,
                        ImageUrl = member?.ImageUrl,
                        LastActive = member.LastActive
                    }
                };

                user.Member.Photos.Add(new Photo
                {
                    Url = member.ImageUrl!,
                    MemberId = member.Id,
                });

                var result = await userManager.CreateAsync(user, "P@ssw0rd");

                if(!result.Succeeded)
                {
                    Console.WriteLine($"Failed to create user {user.DisplayName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new AppUser
            {
                Email = "admin@test.com",
                UserName = "admin@test.com",
                DisplayName = "Admin"
            };

            await userManager.CreateAsync(admin, "P@ssw0rd");
            await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);
        }
    }
}
