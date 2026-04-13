using TheBugTracker.Client;
using TheBugTracker.Models;

namespace TheBugTracker.Interfaces
{
    public interface IProjectRepository
    {
        /// <summary>
        /// Retrieves a project by its id for the current user's company
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Project?> GetProjectByIdAsync(int projectId, UserInfo user);

        /// <summary>
        /// Retrieves all active projects for the current user's company
        /// </summary>
        /// <param name="user>The current user's claims </param>
        Task<IEnumerable<Project>> GetProjectsAsync(UserInfo user);

        /// <summary>
        /// Creates a new project in the database for the user's company
        /// </summary>
        /// <remarks>
        /// Only project managers and admins roles can create new projects
        /// </remarks>
        /// <param name="project">The project to be saved in the database</param>
        /// <param name="user">The current user's claims</param>
        /// <returns>The created project after it has been saved in the database</returns>
        Task<Project> CreateProjectAsync(Project project, UserInfo user);
    }
}