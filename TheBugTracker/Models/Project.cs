using TheBugTracker.Client.Enums;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public ProjectPriority Priority { get; set; }
        public bool IsArchived { get; set; } = false;

        // Navigational Properties
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public virtual ICollection<ApplicationUser> Members { get; set; } = [];
    }
}