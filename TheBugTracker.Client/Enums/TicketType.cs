using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Client.Enums
{
    public enum TicketType
    {
        [Display(Name = "New Development")]
        NewDevelopment,
        [Display(Name = "Work Task")]
        WorkTask,
        Bug,
        [Display(Name = "Change Request")]
        ChangeRequest,
        Enhancement,
        [Display(Name = "General Task")]
        GeneralTask
    }
}