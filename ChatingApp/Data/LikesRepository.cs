using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Helpers;
using ChatingApp.BackEnd.Interfaces;
using ChatingApp.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatingApp.BackEnd.Data
{
    public class LikesRepository(AppDbContext context) : ILikesRepository
    {
        public async Task<IReadOnlyList<string>> GetCurrentMemberLikesIDs(string memberID)
        {
            return await context.Likes
                .Where(l => l.SourceMemberID == memberID)
                .Select(l => l.TargetMemberID)
                .ToListAsync();
        }

        public async Task<MemberLike?> GetMemberLike(string sourceMemberID, string targetMemberID)
        {
            return await context.Likes.FindAsync(sourceMemberID, targetMemberID);
        }

        public async Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likesParams)
        {
            var query = context.Likes.AsQueryable();
            IQueryable<Member> result;

            switch(likesParams.Predicate)
            {
                case "liked":
                    query = query.Where(l => l.SourceMemberID == likesParams.MemberID);
                    result = query.Select(l => l.TargetMember);
                    break;
                case "likedBy":
                    query = query.Where(l => l.TargetMemberID == likesParams.MemberID);
                    result = query.Select(l => l.SourceMember);
                    break;
                default:
                    var likesIDs = await GetCurrentMemberLikesIDs(likesParams.MemberID);

                    result = query
                    .Where(x => x.TargetMemberID == likesParams.MemberID
                        && likesIDs.Contains(x.SourceMemberID))
                    .Select(x => x.SourceMember);

                    break;
            }

            return await PaginationHelper.createPaginationAsync(result, likesParams.PageNumber, likesParams.PageSize);
        }
        public void AddLike(MemberLike memberLike)
        {
            context.Likes.Add(memberLike);
        }

        public void RemoveLike(MemberLike memberLike)
        {
            context.Likes.Remove(memberLike);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }   
    }
}
