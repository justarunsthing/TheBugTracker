using TheBugTracker.Client.Enums;
using System.ComponentModel.DataAnnotations;
using TheBugTracker.Client.Models;

namespace TheBugTracker.Models
{
    public class Ticket
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
        public virtual Project? Project { get; set; }

        [Required]
        public string? SubmitterUserId { get; set; }
        public virtual ApplicationUser? SubmitterUser { get; set; }
        public string? DeveloperUserId { get; set; }
        public virtual ApplicationUser? DeveloperUser { get; set; }
        public virtual ICollection<TicketComment> Comments { get; set; } = [];
        public virtual ICollection<TicketHistory> History { get; set; } = [];
        public virtual ICollection<TicketAttachment> Attachments { get; set; } = [];
    }

    public static class TicketExtensions
    {
        public static TicketDTO ToDTO(this Ticket ticket)
        {
            return new TicketDTO
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Created = ticket.Created,
                Updated = ticket.Updated,
                IsArchived = ticket.IsArchived,
                IsArchivedByProject = ticket.IsArchivedByProject,
                Priority = ticket.Priority,
                Type = ticket.Type,
                Status = ticket.Status,
                ProjectId = ticket.ProjectId,
                Project = ticket.Project?.ToDTO(),
                SubmitterUserId = ticket.SubmitterUserId,
                SubmitterUser = ticket.SubmitterUser?.ToDTO(),
                DeveloperUserId = ticket.DeveloperUserId,
                DeveloperUser = ticket.DeveloperUser?.ToDTO(),
                Attachments = [.. ticket.Attachments.Select(a => a.ToDTO())]
            };
        }
    }
}