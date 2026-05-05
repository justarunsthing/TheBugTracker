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

        /// <summary>
        /// Updates an existing project in the database for the user's company from a ProjectDTO
        /// </summary>
        /// <remarks>
        /// Only project managers and admins roles can update projects, the project manager must be assigned to the project they are trying to update
        /// </remarks>
        /// <param name="project"></param>
        /// <param name="user">The current user's claims</param>
        Task UpdateProjectAsync(ProjectDTO project, UserInfo user);

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
    }
}