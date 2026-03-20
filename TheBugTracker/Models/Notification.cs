using TheBugTracker.Client.Enums;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public class Notification
    {
        private DateTimeOffset _created;

        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Message { get; set; }

        public DateTimeOffset Created 
        { 
            get => _created; 
            set => _created = value.ToUniversalTime(); 
        }

        public NotificationType Type { get; set; }
        public bool HasBeenViewed { get; set; }

        // Navigational Properties
        // Maybe related to a ticket or a project
        public int? TicketId { get; set; }
        public virtual Ticket? Ticket { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }

        [Required]
        public string? RecipientId { get; set; }
        public virtual ApplicationUser? Recipient { get; set; }

        [Required]
        public string? SenderId { get; set; }
        public virtual ApplicationUser? Sender { get; set; }
    }
}