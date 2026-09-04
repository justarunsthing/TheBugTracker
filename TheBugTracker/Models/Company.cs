using TheBugTracker.Client.Models;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? ImageId { get; set; } // FK

        // Navigational Properties
        public virtual FileUpload? Image { get; set; }
        public virtual ICollection<ApplicationUser> Members { get; set; } = [];
        public virtual ICollection<Project> Projects { get; set; } = [];
        public virtual ICollection<Invite> Invites { get; set; } = [];
    }

    public static class CompanyExtensions
    {
        public static CompanyDTO ToDTO(this Company company)
        {
            return new CompanyDTO
            {
                Name = company.Name,
                Description = company.Description,
                ImageUrl = company.ImageId.HasValue 
                    ? $"uploads/{company.ImageId}" 
                    : $"https://api.dicebear.com/9.x/glass/svg?seed={company.Name}",
                Projects = [.. company.Projects.Select(p => p.ToDTO())],
                Members = [.. company.Members.Select(m => m.ToDTO())],
                Invites = [.. company.Invites.Select(i => i.ToDTO())]
            };
        }
    }
}