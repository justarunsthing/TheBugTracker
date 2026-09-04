using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Client.Models
{
    public class InviteDTO
    {
        // Backing field
        private DateTimeOffset _inviteDate;
        private DateTimeOffset? _joinDate;

        [Description("The unique identifier for the invite")]
        public int Id { get; set; }

        [Description("The date and time the invite was sent in UTC")]
        public DateTimeOffset InviteDate
        {
            get => _inviteDate;
            set => _inviteDate = value.ToUniversalTime();
        }

        [Description("The date and time the invitee joined in UTC")]
        public DateTimeOffset? JoinDate
        {
            get => _joinDate;
            set => _joinDate = value?.ToUniversalTime();
        }

        [Required, EmailAddress]
        [Description("The email address of the invitee")]
        public string? InviteeEmail { get; set; }

        [Required]
        [Description("The first name of the invitee")]
        public string? InviteeFirstName { get; set; }

        [Required]
        [Description("The last name of the invitee")]
        public string? InviteeLastName { get; set; }

        [Description("An optional message for the invite")]
        public string? Message { get; set; }

        [Description("Indicates if the invite is valid. Invites will automatically expire after 7 days.")]
        public bool IsValid { get; set; }

        // Navigational Properties

        [Description("The id of the project associated with the invite")]
        public int ProjectId { get; set; }

        [Description("The project associated with the invite")]
        public ProjectDTO? Project { get; set; }

        [Required]
        [Description("The id of the admin who sent the invite")]
        public string? InvitorId { get; set; }

        [Description("The admin who sent the invite")]
        public UserDTO? Invitor { get; set; }

        [Description("The id of the user who received the invite")]
        public string? InviteeId { get; set; }

        [Description("The user who received the invite")]
        public UserDTO? Invitee { get; set; }
    }
}