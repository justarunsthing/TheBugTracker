using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using TheBugTracker.Client;
using TheBugTracker.Models;

namespace TheBugTracker.Components.Account
{
    public class CustomUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
                                                  RoleManager<IdentityRole> roleManager,
                                                  IOptions<IdentityOptions> options)
        : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(userManager, roleManager, options)
    {
        // Method gets called automatically when a user logs in and generates the claims for that user
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);

            string profilePictureUrl = user.ProfilePictureId.HasValue
                ? $"uploads/{user.ProfilePictureId}"
                : $"https://api.dicebear.com/9.x/glass/svg?seed={user.Id}";

            List <Claim> customClaims =
            [
                new Claim(nameof(UserInfo.FirstName), user.FirstName!),
                new Claim(nameof(UserInfo.LastName), user.LastName!),
                new Claim(nameof(UserInfo.CompanyId), user.CompanyId.ToString()!),
                new Claim(nameof(UserInfo.ProfilePictureUrl), profilePictureUrl)
            ];

            identity.AddClaims(customClaims);

            return identity;
        }
    }
}