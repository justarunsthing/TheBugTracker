using TheBugTracker.Client.Models;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Client.Services
{
    public class WASMProjectDTOService(HttpClient http) : IProjectDTOService
    {
        public Task<ProjectDTO?> GetProjectByIdAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProjectDTO>> GetProjectsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectDTO> CreateProjectAsync(ProjectDTO project, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateProjectAsync(ProjectDTO project, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task ArchiveProjectAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task RestoreProjectAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}