using TheBugTracker.Client;
using TheBugTracker.Models;
using TheBugTracker.Client.Enums;

namespace TheBugTracker.Interfaces
{
    public interface ICompanyRepository
    {
        /// <summary>
        /// Get all users in the current user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<ApplicationUser>> GetUsersAsync(UserInfo userInfo);

        /// <summary>
        /// Get all users in the current user's company in a specific role
        /// </summary>
        /// <param name="role">The role assigned to the users</param>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(Role role, UserInfo userInfo);

        /// <summary>
        /// Get detailed information about the current user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<Company> GetCompanyAsync(UserInfo userInfo);

        /// <summary>
        /// Updates a company's details in the database if the user is the admin of the company
        /// </summary>
        /// <param name="company">The company details to update</param>
        /// <param name="userInfo">The current user's claims</param>
        Task UpdateCompanyAsync(Company company, UserInfo userInfo);

        /// <summary>
        /// Assigns a user to a new role, removing them from any previous ones.
        /// Only admins may reassign user's roles within their company. Demo users may never be assigned.
        /// </summary>
        /// <param name="userId">The id of the user to assign a new role</param>
        /// <param name="newRole">The role to assign to the user</param>
        /// <param name="userInfo">The current user's claims</param>
        /// <returns></returns>
        Task AssignUserRoleAsync(string userId, Role newRole, UserInfo userInfo);
    }
}