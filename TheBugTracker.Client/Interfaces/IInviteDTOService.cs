using TheBugTracker.Client.Models;

namespace TheBugTracker.Client.Interfaces
{
    public interface IInviteDTOService
    {
        /// <summary>
        /// Saves a new invite for the user's company to the database. Only admins may create invites.
        /// </summary>
        /// <param name="invite">The details of the invite to create</param>
        /// <param name="userInfo">The current user's claims</param>
        /// <returns>The created invite</returns>
        Task<InviteDTO> CreateInviteAsync(InviteDTO dto, UserInfo userInfo);
    }
}