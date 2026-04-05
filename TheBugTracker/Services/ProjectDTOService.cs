using TheBugTracker.Models;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Models;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Services
{
    public class ProjectDTOService(IProjectRepository repository) : IProjectDTOService
    {
        public async Task<IEnumerable<ProjectDTO>> GetProjectsAsync(string userId)
        {
            IEnumerable<Project> projects = await repository.GetProjectsAsync(userId);
            IEnumerable<ProjectDTO> dtos = projects.Select(p => p.ToDTO());

            return dtos;
        }
    }
}