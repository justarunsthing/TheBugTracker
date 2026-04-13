using TheBugTracker.Client.Models;

namespace TheBugTracker.Client.Interfaces
{
    public interface IProjectDTOService
    {
        /// <summary>
        /// Retrieves a project by its id for the current user's company
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ProjectDTO?> GetProjectByIdAsync(int projectId, UserInfo user);

        /// <summary>
        /// Retrieves all active projects for the current user's company
        /// </summary>
        /// <param name="user">The current user's claims</param>
        Task<IEnumerable<ProjectDTO>> GetProjectsAsync(UserInfo user);

        /// <summary>
        /// Creates a new project in the database for the user's company from a ProjectDTO
        /// </summary>
        /// <remarks>
        /// Only project managers and admins roles can create new projects
        /// </remarks>
        /// <param name="project">The project to be saved in the database</param>
        /// <param name="user">The current user's claims</param>
        /// <returns>The created project's DTO after it has been saved in the database</returns>
        Task<ProjectDTO> CreateProjectAsync(ProjectDTO project, UserInfo user);
    }
}