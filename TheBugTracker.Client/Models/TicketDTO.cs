using System.ComponentModel;
using TheBugTracker.Client.Enums;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Client.Models
{
    public class TicketDTO
    {
        private DateTimeOffset _created;
        private DateTimeOffset? _updated;

        [Description("The unique identifier of the ticket")]
        public int Id { get; set; }

        [Required]
        [Description("The title of the ticket")]
        public string? Title { get; set; }

        [Required]
        [Description("The detailed description of the ticket")]
        public string? Description { get; set; }

        [Description("The date and time the ticket was created in UTC")]
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        [Description("The date and time the ticket was last updated in UTC after creation")]
        public DateTimeOffset? Updated
        {
            get => _updated;
            set => _updated = value?.ToUniversalTime();
        }

        [Description("Indicates whether the ticket is currenty archived")]
        public bool IsArchived { get; set; } = false;

        [Description("Indicates whether the ticket is archived as a result of archiving its project")]
        public bool IsArchivedByProject { get; set; } = false;

        [Description("The priority level of the ticket")]
        public TicketPriority Priority { get; set; }

        [Description("The type of task described by the ticket")]
        public TicketType Type { get; set; }

        [Description("The current status of the ticket's task")]
        public TicketStatus Status { get; set; }

        // Navigational Properties
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid project")]
        [Description("The Id of the project the ticket belongs to")]
        public int ProjectId { get; set; }

        [Description("The details of the project this ticket belongs to")]
        public ProjectDTO? Project { get; set; }

        [Required]
        [Description("The Id of the user who created this ticket")]
        public string? SubmitterUserId { get; set; }

        [Description("The details of the user who created this ticket")]
        public UserDTO? SubmitterUser { get; set; }

        [Description("The Id of the developer this ticket is assigned to, if any")]
        public string? DeveloperUserId { get; set; }

        [Description("The details of the user this ticket is assigned to, if any")]
        public UserDTO? DeveloperUser { get; set; }

        [Description("Files uploaded in support of this ticket")]
        public ICollection<TicketAttachmentDTO> Attachments { get; set; } = [];

        [Description("Comments that have been left on this ticket")]
        public ICollection<TicketCommentDTO> Comments { get; set; } = [];

        [Description("A history of events related to this ticket")]
        public ICollection<TicketHistoryDTO> History { get; set; } = [];

        [JsonIgnore]
        public DateTimeOffset LastModified => Updated ?? Created;
    }
}