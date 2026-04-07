using TheBugTracker.Client.Models;

namespace TheBugTracker.Client.Interfaces
{
    public interface IProjectDTOService
    {
        /// <summary>
        /// Retrieves all active projects for the current user's company
        /// </summary>
        /// <param name="user">The current user's claims</param>
        Task<IEnumerable<ProjectDTO>> GetProjectsAsync(UserInfo user);
    }
}