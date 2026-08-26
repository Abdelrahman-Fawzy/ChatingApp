using System.Security.Claims;

namespace ChatingApp.BackEnd.Extensions
{
    public static class ClaimsPrincipleExtension
    {
        public static string GetMemberId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Cannot get memberId from token");
        }
    }
}
