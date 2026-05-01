using TheBugTracker.Client.Enums;
using TheBugTracker.Client.Models;

namespace TheBugTracker.Client.Interfaces
{
    public interface ICompanyDTOService
    {
        /// <summary>
        /// Get all users in the current user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<UserDTO>> GetUsersAsync(UserInfo userInfo);

        /// <summary>
        /// Get all users in the current user's company in a specific role
        /// </summary>
        /// <param name="role">The role assigned to the users</param>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<UserDTO>> GetUsersInRoleAsync(Role role, UserInfo userInfo);
    }
}