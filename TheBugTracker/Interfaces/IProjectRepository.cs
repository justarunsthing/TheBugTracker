using TheBugTracker.Client;
using TheBugTracker.Models;

namespace TheBugTracker.Interfaces
{
    public interface IProjectRepository
    {
        /// <summary>
        /// Retrieves all active projects for the current user's company
        /// </summary>
        /// <param name="user>The current user's claims </param>
        Task<IEnumerable<Project>> GetProjectsAsync(UserInfo user);
    }
}