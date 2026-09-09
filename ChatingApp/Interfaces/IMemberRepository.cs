using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Helpers;
using ChatingApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatingApp.BackEnd.Interfaces
{
    public interface IMemberRepository
    {
        public Task<PaginatedResult<Member>> GetMembersAsync(MemberParams memberParams);
        public Task<Member?> GetMember(string id);
        public Task<Member?> GetMemberToUpdate(string id);
        public Task<IReadOnlyList<Photo>> GetPhotosByMemberIdAsync(string id);
        public Task<bool> SaveChangesAsync();
        public void Update(Member member);

    }
}
