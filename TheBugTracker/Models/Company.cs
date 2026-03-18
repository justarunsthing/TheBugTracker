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
        public virtual ImageUpload? Image { get; set; }
        public virtual ICollection<ApplicationUser> Members { get; set; } = [];
    }
}