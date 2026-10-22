using System.Security.Claims;

namespace Jexkey.WebAPI.Extensions {
    public static class ClaimsPrincipalExtensions {
        public static Guid GetUserId(this ClaimsPrincipal user) {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var guid)) {
                return guid;
            }

            throw new UnauthorizedAccessException("Token does not contain a valid User ID.");
        }
    }
}
