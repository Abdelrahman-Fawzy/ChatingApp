using ChatingApp.BackEnd.Data;
using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Extensions;
using ChatingApp.BackEnd.Helpers;
using ChatingApp.BackEnd.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatingApp.BackEnd.Controllers
{
    public class LikesController(IUnitOfWork uow) : BaseApiController
    {

        [HttpPost("{targetMemberID}")]
        public async Task<ActionResult> ToggleLike(string targetMemberID)
        {
            var sourceMemberID = User.GetMemberId();
            if(sourceMemberID == targetMemberID) return BadRequest(new { Message = "لا يمكنك إضافة نفسك إلى قائمة الإعجابات" });

            var existingLike = await uow.LikesRepository.GetMemberLike(sourceMemberID, targetMemberID);

            if(existingLike == null)
            {
                var like = new MemberLike
                {
                    SourceMemberID = sourceMemberID,
                    TargetMemberID = targetMemberID
                };

                uow.LikesRepository.AddLike(like);
            } else
            {
                uow.LikesRepository.RemoveLike(existingLike);
            }

            if(await uow.CompleteAsync()) return Ok();

            return BadRequest(new { Message = "فشل في إضافة الإعجاب" });
        }

        [HttpGet("list")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetCurrentMemberLikesIDs()
        {
            var memberID = User.GetMemberId();
            var likesIDs = await uow.LikesRepository.GetCurrentMemberLikesIDs(memberID);
            return Ok(likesIDs);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<Member>>> GetMemberLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.MemberID = User.GetMemberId();
            var members = await uow.LikesRepository.GetMemberLikes(likesParams);

            return Ok(members);
        }
    }
}
