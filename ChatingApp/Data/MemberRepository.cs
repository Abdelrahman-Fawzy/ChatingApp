using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Helpers;
using ChatingApp.BackEnd.Interfaces;
using ChatingApp.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ChatingApp.BackEnd.Data
{
    public class MemberRepository(AppDbContext _context) : IMemberRepository
    {
        public async Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams)
        {
            var query = _context.Members.AsQueryable();

            query = query.Where(m => m.Id != memberParams.CurrentMemberID);

            if (!string.IsNullOrEmpty(memberParams.Gender))
            {
                query = query.Where(m => m.Gender == memberParams.Gender);
            }

            var minDateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MaxAge - 1));
            var maxDateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-memberParams.MinAge));
            query = query.Where(m => m.DateOfBirth >= minDateOfBirth && m.DateOfBirth <= maxDateOfBirth);

            query = memberParams.OrderBy switch
            {
                "created" => query.OrderByDescending(m => m.Created),
                _ => query.OrderByDescending(m => m.LastActive)
            };

            return await PaginationHelper.createPaginationAsync(query, memberParams.PageNumber, memberParams.PageSize);
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

        public async Task<Member?> GetMemberWithAllPhotosToUpdate(string id)
        {
            return await _context.Members
                .Include(m => m.User)
                .Include(m => m.Photos)
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IReadOnlyList<Photo>> GetPhotosByMemberIdAsync(string memberId, bool isCurrentUser)
        {
            var query = _context.Members
                .Where(m => m.Id == memberId)
                .SelectMany(m => m.Photos);


            if (isCurrentUser) query = query.IgnoreQueryFilters();

            return await query.ToListAsync();

        }

        public void Update(Member member)
        {
            _context.Entry(member).State = EntityState.Modified;
        }
    }
}
