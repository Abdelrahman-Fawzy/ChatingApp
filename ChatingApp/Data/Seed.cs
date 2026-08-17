using ChatingApp.BackEnd.DTOs;
using ChatingApp.BackEnd.Entities;
using ChatingApp.Data;
using ChatingApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ChatingApp.BackEnd.Data
{
    public class Seed
    {
        public static async Task SeedUsers(AppDbContext context)
        {
            if (await context.Users.AnyAsync()) return;

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
                    DisplayName = member.DisplayName,
                    Email = member.Email,
                    ImageUrl = member.ImageUrl,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("P@ssw0rd")),
                    PasswordSalt = hmac.Key,
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

                context.Users.Add(user);

                await context.SaveChangesAsync();
            }
        }
    }
}
