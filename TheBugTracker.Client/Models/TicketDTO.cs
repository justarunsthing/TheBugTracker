using TheBugTracker.Client.Enums;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Client.Models
{
    public class TicketDTO
    {
        private DateTimeOffset _created;
        private DateTimeOffset? _updated;

        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        public DateTimeOffset? Updated
        {
            get => _updated;
            set => _updated = value?.ToUniversalTime();
        }

        public bool IsArchived { get; set; } = false;
        public bool IsArchivedByProject { get; set; } = false;
        public TicketPriority Priority { get; set; }
        public TicketType Type { get; set; }
        public TicketStatus Status { get; set; }

        // Navigational Properties
        public int ProjectId { get; set; }
        public ProjectDTO? Project { get; set; }

        [Required]
        public string? SubmitterUserId { get; set; }
        public UserDTO? SubmitterUser { get; set; }
        public string? DeveloperUserId { get; set; }
        public UserDTO? DeveloperUser { get; set; }
        public ICollection<TicketAttachmentDTO> Attachments { get; set; } = [];
        public ICollection<TicketCommentDTO> Comments { get; set; } = [];
        public ICollection<TicketHistoryDTO> History { get; set; } = [];

        [JsonIgnore]
        public DateTimeOffset LastModified => Updated ?? Created;
    }
}