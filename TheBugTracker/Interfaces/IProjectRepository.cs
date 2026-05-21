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
        /// <param name="user">The current user's claims </param>
        Task<IEnumerable<Project>> GetProjectsAsync(UserInfo user);

        /// <summary>
        /// Retrieves all archived projects for the current user's company
        /// </summary>
        /// <param name="user">The current user's claims</param>
        Task<IEnumerable<Project>> GetArchivedProjectsAsync(UserInfo user);

        /// <summary>
        /// Retrieves a list of users currently assigned to the project
        /// </summary>
        /// <param name="projectId">The Id of the project</param>
        /// <param name="user">The current user's claims</param>
        /// <returns>A collection of users</returns>
        Task<IEnumerable<ApplicationUser>> GetProjectMembersAsync(int projectId, UserInfo user);

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

        /// <summary>
        /// Updates an existing project in the database for the user's company from a ProjectDTO
        /// </summary>
        /// <remarks>
        /// Only project managers and admins roles can update projects, the project manager must be assigned to the project they are trying to update
        /// </remarks>
        /// <param name="project"></param>
        /// <param name="user">The current user's claims</param>
        Task UpdateProjectAsync(Project project, UserInfo user);

        /// <summary>
        /// Archives a project to mark it as inactive. This method will also archive all of the tickets associated with the project
        /// </summary>
        /// <remarks>
        /// Projects may only be archived by the admins or project managers assigned to the project
        /// </remarks>
        /// <param name="projectId"></param>
        /// <param name="user">The current user's claims</param>
        Task ArchiveProjectAsync(int projectId, UserInfo user);

        /// <summary>
        /// Restores a project to mark it as active. This method will also restore all of the tickets associated with the project
        /// that were not previously archived
        /// </summary>
        /// <remarks>
        /// Projects may only be restored by the admins or project managers assigned to the project
        /// </remarks>
        /// <param name="projectId"></param>
        /// <param name="user">The current user's claims</param>
        Task RestoreProjectAsync(int projectId, UserInfo user);

        /// <summary>
        /// Assigns a user to the specified project, if they are not already assigned
        /// </summary>
        /// <param name="projectId">The Id of the project</param>
        /// <param name="userId">The Id of the user</param>
        /// <param name="user">The current user's claims</param>
        Task AddProjectMemberAsync(int projectId, string userId, UserInfo user);

        /// <summary>
        /// Removes a user from the specified project, if they are currently assigned
        /// </summary>
        /// <param name="projectId">The Id of the project</param>
        /// <param name="userId">The Id of the user</param>
        /// <param name="user">The current user's claims</param>
        Task RemoveProjectMemberAsync(int projectId, string userId, UserInfo user);

        /// <summary>
        /// Retrieves the assigned project manager for the project
        /// </summary>
        /// <param name="projectId">The Id of the project</param>
        /// <param name="user">The current user's claims</param>
        /// <returns>The assigned project manager or null</returns>
        Task<ApplicationUser?> GetProjectManagerAsync(int projectId, UserInfo user);

        /// <summary>
        /// Assigns a project manager to a project, removes if another project manager was already assigned
        /// </summary>
        /// <param name="projectId">The id of the project</param>
        /// <param name="managerId">The id of the project manager to assign</param>
        /// <param name="user">The current user's claims</param>
        Task AssignProjectManagerAsync(int projectId, string managerId, UserInfo user);

        /// <summary>
        /// Removes the project manager from the project is assigned
        /// </summary>
        /// <param name="projectId">The id of the project</param>
        /// <param name="user">The current user's claims</param>
        Task RemoveProjectManagerAsync(int projectId, UserInfo user);
    }
}