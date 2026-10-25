using ChatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatingApp.BackEnd.Controllers
{
    public class AdminController(UserManager<AppUser> userManager) : BaseApiController
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
        public ActionResult GetPhotosForModeration()
        {
            return Ok("Admins or moderators can see this");
        }
    }
}
