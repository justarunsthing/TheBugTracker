using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TheBugTracker.Client.Enums;
using TheBugTracker.Client.Models;

namespace TheBugTracker.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        public Guid? ProfilePictureId { get; set; }
        public virtual FileUpload? ProfilePicture { get; set; }

        // Navigational Property
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public virtual ICollection<Project> Projects { get; set; } = [];
    }

    public static class ApplicationUserExtensions
    {
        public static UserDTO ToDTO(this ApplicationUser user)
        {
            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                ImageUrl = user.ProfilePictureId.HasValue 
                    ? $"uploads/{user.ProfilePictureId}" 
                    : $"https://api.dicebear.com/9.x/glass/svg?seed={user.FirstName}{user.LastName}"
            };
        }

        public static async Task<UserDTO> ToDTOWithRole(this ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            UserDTO dto = user.ToDTO();
            var roleNames = await userManager.GetRolesAsync(user);
            string? roleName = roleNames
                .Where(rn => rn != nameof(Role.DemoUser))
                .FirstOrDefault();

            bool success = Enum.TryParse(roleName, out Role role);

            if (success)
            {
                dto.Role = role;
            }

            return dto;
        }
    }
}