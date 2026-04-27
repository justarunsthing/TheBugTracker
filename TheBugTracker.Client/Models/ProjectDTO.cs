using TheBugTracker.Client.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace TheBugTracker.Client.Models
{
    public class ProjectDTO
    {
        // Fields
        private DateTimeOffset _created;
        private DateTimeOffset? _startDate;
        private DateTimeOffset? _endDate;

        [Description("The unique identifier for the project")]
        public int Id { get; set; }

        [Required]
        [Description("The name or title of the project")]
        public string? Name { get; set; }

        [Required]
        [Description("A brief summary or details about the project")]
        public string? Description { get; set; }

        // From auto property to full property
        [Description("The date and time when the project was created, stored in UTC")]
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        [Required]
        [Description("The date and time when the project is scheduled to start, stored in UTC")]
        public DateTimeOffset? StartDate
        {
            get => _startDate;
            set => _startDate = value?.ToUniversalTime();
        }

        [Required]
        [Description("The date and time when the project is scheduled to end, stored in UTC")]
        public DateTimeOffset? EndDate
        {
            get => _endDate;
            set => _endDate = value?.ToUniversalTime();
        }

        [Description("The relative priority assigned to a project")]
        public ProjectPriority Priority { get; set; }

        [Description("Indicates whether the project is active or archived")]
        public bool IsArchived { get; set; } = false;

        [Description("The collection of tickets associated with the project")]
        public ICollection<TicketDTO> Tickets { get; set; } = [];

        [Description("The collection of users associated with the project")]
        public ICollection<UserDTO> Members { get; set; } = [];

        #region Helper properties

        [Required, JsonIgnore]
        public DateTime? StartDateTime 
        { 
            get => StartDate?.DateTime;
            set => StartDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
        }

        [Required, JsonIgnore]
        public DateTime? EndDateTime
        {
            get => EndDate?.DateTime;
            set => EndDate = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
        }

        #endregion
    }
}