using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace TheBugTracker.Client.Helpers
{
    public static class UserInfoHelper
    {
        public static async Task<UserInfo?> GetUserInfoAsync(Task<AuthenticationState>? authStateTask)
        {
            if (authStateTask is null)
            {
                return null;
            }

            AuthenticationState authState = await authStateTask;
            ClaimsPrincipal user = authState.User;

            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                var email = user.FindFirst(ClaimTypes.Email)!.Value;
                var firstName = user.FindFirst(nameof(UserInfo.FirstName))!.Value;
                var lastName = user.FindFirst(nameof(UserInfo.LastName))!.Value;
                var companyId = user.FindFirst(nameof(UserInfo.CompanyId))!.Value;
                var profilePictureUrl = user.FindFirst(nameof(UserInfo.ProfilePictureUrl))!.Value;
                var roles = user.FindAll(nameof(UserInfo.Roles)).Select(r => r.Value);

                return new UserInfo
                {
                    UserId = userId,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    CompanyId = int.Parse(companyId),
                    ProfilePictureUrl = profilePictureUrl,
                    Roles = [.. roles]
                };
            }
            catch
            {
                return null;
            }
        }
    }
}