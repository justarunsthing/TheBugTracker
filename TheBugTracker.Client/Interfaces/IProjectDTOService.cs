using TheBugTracker.Client.Models;

namespace TheBugTracker.Client.Interfaces
{
    public interface IProjectDTOService
    {
        /// <summary>
        /// Retrieves all projects in the database
        /// </summary>
        /// <returns>An enumerable of projects</returns>
        Task<IEnumerable<ProjectDTO>> GetProjectsAsync();
    }
}