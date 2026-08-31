using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Interfaces;
using ChatingApp.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ChatingApp.BackEnd.Data
{
    public class MemberRepository(AppDbContext _context) : IMemberRepository
    {
        public async Task<IReadOnlyList<Member>> GetMembersAsync()
        {
            return await _context.Members.ToListAsync();
        }

        public async Task<Member?> GetMember(string id)
        {
            return await _context.Members.FindAsync(id);
        }

        public async Task<Member?> GetMemberToUpdate(string id)
        {
            return await _context.Members
                .Include(m => m.User)
                .Include(m => m.Photos)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IReadOnlyList<Photo>> GetPhotosByMemberIdAsync(string id)
        {
            return await _context.Members
                .Where(m => m.Id == id)
                .SelectMany(m => m.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(Member member)
        {
            _context.Entry(member).State = EntityState.Modified;
        }
    }
}
