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

        /// <summary>
        /// Get detailed information about the current user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<CompanyDTO> GetCompanyAsync(UserInfo userInfo);

        /// <summary>
        /// Updates a company's details in the database if the user is the admin of the company
        /// </summary>
        /// <param name="company">The company details to update</param>
        /// <param name="userInfo">The current user's claims</param>
        Task UpdateCompanyAsync(CompanyDTO company, UserInfo userInfo);
    }
}