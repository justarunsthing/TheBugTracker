using TheBugTracker.Client;
using TheBugTracker.Models;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Models;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Services
{
    public class ProjectDTOService(IProjectRepository repository) : IProjectDTOService
    {
        public async Task<ProjectDTO?> GetProjectByIdAsync(int projectId, UserInfo user)
        {
            Project? project = await repository.GetProjectByIdAsync(projectId, user);

            return project?.ToDTO();
        }

        public async Task<IEnumerable<ProjectDTO>> GetProjectsAsync(UserInfo user)
        {
            IEnumerable<Project> projects = await repository.GetProjectsAsync(user);
            IEnumerable<ProjectDTO> dtos = projects.Select(p => p.ToDTO());

            return dtos;
        }

        public async Task<ProjectDTO> CreateProjectAsync(ProjectDTO project, UserInfo user)
        {
            Project dbProject = new()
            {
                Name = project.Name,
                Description = project.Description,
                Created = DateTimeOffset.UtcNow,
                StartDate = project.StartDate ?? DateTimeOffset.UtcNow,
                EndDate = project.EndDate ?? DateTimeOffset.UtcNow + TimeSpan.FromDays(7),
                Priority = project.Priority,
                IsArchived = false,
                CompanyId = user.CompanyId,
            };

            dbProject = await repository.CreateProjectAsync(dbProject, user);

            return dbProject.ToDTO();
        }

        public async Task UpdateProjectAsync(ProjectDTO project, UserInfo user)
        {
            Project? dbProject = await repository.GetProjectByIdAsync(project.Id, user);

            if (dbProject is null)
            {
                return;
            }

            dbProject.Name = project.Name;
            dbProject.Description = project.Description;
            dbProject.StartDate = project.StartDate ?? dbProject.StartDate;
            dbProject.EndDate = project.EndDate ?? dbProject.EndDate;
            dbProject.Priority = project.Priority;

            await repository.UpdateProjectAsync(dbProject, user);
        }

        public async Task ArchiveProjectAsync(int projectId, UserInfo user)
        {
            await repository.ArchiveProjectAsync(projectId, user);
        }

        public async Task RestoreProjectAsync(int projectId, UserInfo user)
        {
            await repository.RestoreProjectAsync(projectId, user);
        }

        public async Task AddProjectMemberAsync(int projectId, string userId, UserInfo user)
        {
            await repository.AddProjectMemberAsync(projectId, userId, user);
        }

        public async Task RemoveProjectMemberAsync(int projectId, string userId, UserInfo user)
        {
            await repository.RemoveProjectMemberAsync(projectId, userId, user);
        }
    }
}