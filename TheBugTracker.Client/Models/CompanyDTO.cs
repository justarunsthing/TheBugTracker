using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Client.Models
{
    public class CompanyDTO
    {
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string ImageUrl { get; set; } = $"https://api.dicebear.com/9.x/glass/svg?seed={Random.Shared.Next()}";
        public ICollection<ProjectDTO> Projects { get; set; } = [];

        //public ICollection<ApplicationUser> Members { get; set; } = [];
        //public ICollection<Invite> Invites { get; set; } = [];
    }
}