using System.Text.Json.Serialization;

namespace TheBugTracker.Client.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TicketsFilter
    {
        Open,
        Resolved,
        Assigned,
        Archived
    }
}