using TheBugTracker.Client.Enums;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Client.Models
{
    public class ProjectDTO
    {
        // Fields
        private DateTimeOffset _created;
        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;

        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        // From auto property to full property
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        public DateTimeOffset StartDate
        {
            get => _startDate;
            set => _startDate = value.ToUniversalTime();
        }

        public DateTimeOffset EndDate
        {
            get => _endDate;
            set => _endDate = value.ToUniversalTime();
        }

        public ProjectPriority Priority { get; set; }
        public bool IsArchived { get; set; } = false;
        //public ICollection<ApplicationUser> Members { get; set; } = [];
        //public ICollection<Ticket> Tickets { get; set; } = [];
    }
}