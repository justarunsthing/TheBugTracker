using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Client.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(Enum value)
        {
            string? displayName = value.GetType()
                .GetMember(value.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName();

            if (string.IsNullOrEmpty(displayName))
            {
                return value.ToString();
            }
            else
            {
                return displayName;
            }
        }
    }
}