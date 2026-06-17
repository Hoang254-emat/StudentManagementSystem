using MyWebApi.Services.Interfaces;
using System.Security.Claims;

namespace MyWebApi.Services.Implementations
{
    public class UserAccessor(IHttpContextAccessor httpContextAccessor) : IUserAccessor
    {
        public string GetCurrentUserId() => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        public string GetCurrentRole() => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
}
