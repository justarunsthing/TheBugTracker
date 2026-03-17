using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Client.Enums
{
    public enum TicketStatus
    {
        New,
        [Display(Name = "In Development")]
        InDevelopment,
        Testing,
        Resolved
    }
}