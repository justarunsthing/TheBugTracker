using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheBugTracker.Client.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
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