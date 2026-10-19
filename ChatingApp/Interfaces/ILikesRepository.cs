using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Helpers;

namespace ChatingApp.BackEnd.Interfaces
{
    public interface ILikesRepository
    {
        Task<MemberLike?> GetMemberLike(string sourceMemberID, string targetMemberID);
        Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likesParams);
        Task<IReadOnlyList<string>> GetCurrentMemberLikesIDs(string memberID);
        void AddLike(MemberLike memberLike);
        void RemoveLike(MemberLike memberLike);
        Task<bool> SaveAllAsync();
    }
}
