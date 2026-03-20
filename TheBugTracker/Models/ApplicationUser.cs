using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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
}