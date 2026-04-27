using System.Text.Json.Serialization;

namespace TheBugTracker.Client.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProjectPriority
    {
        Low,
        Medium,
        High,
        Urgent
    }
}