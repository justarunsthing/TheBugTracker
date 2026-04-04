using TheBugTracker.Models;

namespace TheBugTracker.Interfaces
{
    public interface IProjectRepository
    {
        /// <summary>
        /// Retrieves all projects in the database
        /// </summary>
        Task<IEnumerable<Project>> GetProjectsAsync();
    }
}