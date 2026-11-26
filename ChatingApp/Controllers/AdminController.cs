using ChatingApp.BackEnd.Entities;
using ChatingApp.BackEnd.Interfaces;
using ChatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatingApp.BackEnd.Controllers
{
    public class AdminController(UserManager<AppUser> userManager, IUnitOfWork uow, IPhotoService photoService) : BaseApiController
    {
        [Authorize(Policy = "RequireAdminModeratorRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await userManager.Users.ToListAsync();
            var usersList = new List<Object>();

            foreach(var user in users)
            {
                var userRoles = await userManager.GetRolesAsync(user);
                usersList.Add(new
                {
                    user.Id,
                    user.UserName,
                    Roles = userRoles
                });
            }

            return Ok(usersList);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{userId}")]
        public async Task<ActionResult<IList<string>>> editUserRoles(string userId, [FromQuery]string roles)
        {
            if (String.IsNullOrEmpty(roles)) return BadRequest(new { Message = "يجب أختيار صلاحية واحدة على الأقل" });

            var selectedRoles = roles.Split(',').ToArray();

            var user = await userManager.FindByIdAsync(userId);

            if (user == null) return NotFound(new { Message = "هذا المستحدم غير موجود" });

            var userRoles = await userManager.GetRolesAsync(user);

            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if(!result.Succeeded) return BadRequest(new { Message = "فشل أضافة الصلاحيات" });

            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if(!result.Succeeded) return BadRequest(new { Message = "فشل أزالة الصلاحيات" });

            return Ok(await userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult<IEnumerable<Photo>>> GetPhotosForModeration()
        {
            return Ok(await uow.PhotoRepository.GetUnApprovedPhotos());
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId)
        {
            var photo = await uow.PhotoRepository.GetPhotoById(photoId);

            if(photo == null) return NotFound(new { Message = "الصورة غير موجودة" });

            var memebr = await uow.MemberRepository.GetMemberToUpdate(photo.MemberId);

            if(memebr == null) return NotFound(new { Message = "المستخدم غير موجود" });

            photo.IsApproved = true;

            if(memebr.ImageUrl == null)
            {
                memebr.ImageUrl = photo.Url;
                memebr.User.ImageUrl = photo.Url;
            }

            await uow.CompleteAsync();

            return Ok(new { Message = "تم الموافقة على الصورة" });
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
            var photo = await uow.PhotoRepository.GetPhotoById(photoId);

            if (photo == null) return NotFound(new { Message = "الصورة غير موجودة" });

            if(photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAync(photo.PublicId);

                if(result.Result == "ok")
                {
                    uow.PhotoRepository.RemovePhoto(photo);
                }
            } else
            {
                uow.PhotoRepository.RemovePhoto(photo);
            }

            await uow.CompleteAsync();

            return Ok(new { Message = "تم رفض الصورة" });
        }
    }
}
