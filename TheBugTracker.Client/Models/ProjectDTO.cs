using TheBugTracker.Client.Enums;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Client.Models
{
    public class ProjectDTO
    {
        // Fields
        private DateTimeOffset _created;
        private DateTimeOffset? _startDate;
        private DateTimeOffset? _endDate;

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

        [Required]
        public DateTimeOffset? StartDate
        {
            get => _startDate;
            set => _startDate = value?.ToUniversalTime();
        }

        [Required]
        public DateTimeOffset? EndDate
        {
            get => _endDate;
            set => _endDate = value?.ToUniversalTime();
        }

        public ProjectPriority Priority { get; set; }
        public bool IsArchived { get; set; } = false;
        public ICollection<TicketDTO> Tickets { get; set; } = [];
        public ICollection<UserDTO> Members { get; set; } = [];

        #region Helper properties

        [Required]
        public DateTime? StartDateTime 
        { 
            get => StartDate?.DateTime;
            set => StartDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
        }

        [Required]
        public DateTime? EndDateTime
        {
            get => EndDate?.DateTime;
            set => EndDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
        }

        #endregion
    }
}