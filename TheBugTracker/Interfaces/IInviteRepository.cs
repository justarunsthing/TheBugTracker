using TheBugTracker.Client;
using TheBugTracker.Models;

namespace TheBugTracker.Interfaces
{
    public interface IInviteRepository
    {
        /// <summary>
        /// Gets all invites for the current user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        /// <returns>The invites for the user's company</returns>
        Task<IEnumerable<Invite>> GetInvitesAsync(UserInfo userInfo);

        /// <summary>
        /// Saves a new invite for the user's company to the database. Only admins may create invites.
        /// </summary>
        /// <param name="invite">The details of the invite to create</param>
        /// <param name="userInfo">The current user's claims</param>
        /// <returns>The created invite</returns>
        Task<Invite> CreateInviteAsync(Invite invite, UserInfo userInfo);
    }
}