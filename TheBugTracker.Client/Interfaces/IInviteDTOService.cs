using TheBugTracker.Client.Models;

namespace TheBugTracker.Client.Interfaces
{
    public interface IInviteDTOService
    {
        /// <summary>
        /// Gets all invites for the current user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        /// <returns>The invites for the user's company</returns>
        Task<IEnumerable<InviteDTO>> GetInvitesAsync(UserInfo userInfo);

        /// <summary>
        /// Saves a new invite for the user's company to the database. Only admins may create invites.
        /// </summary>
        /// <param name="invite">The details of the invite to create</param>
        /// <param name="userInfo">The current user's claims</param>
        /// <returns>The created invite</returns>
        Task<InviteDTO> CreateInviteAsync(InviteDTO dto, UserInfo userInfo);

        /// <summary>
        /// Invalidates a previously created invite. Only admins may cancel invites.
        /// </summary>
        /// <param name="inviteId">The ID of the invite to cancel</param>
        /// <param name="userInfo">The current user's claims</param>
        Task CancelInviteAsync(int inviteId, UserInfo userInfo);
    }
}