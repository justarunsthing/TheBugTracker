using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Client.Models
{
    public class TicketHistoryDTO
    {
        private DateTimeOffset _created;

        [Required]
        public string? Description { get; set; }
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        // Navigational Properties
        [Required]
        public string? UserId { get; set; }
        public UserDTO? User { get; set; }
    }
}