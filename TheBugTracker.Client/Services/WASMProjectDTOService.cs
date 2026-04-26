using System.Net.Http.Json;
using TheBugTracker.Client.Models;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Client.Services
{
    public class WASMProjectDTOService(HttpClient http) : IProjectDTOService
    {
        public async Task<ProjectDTO?> GetProjectByIdAsync(int projectId, UserInfo user)
        {
            try
            {
                var project = await http.GetFromJsonAsync<ProjectDTO>($"api/projects/{projectId}"); 

                return project;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return null;
            }
        }

        public async Task<IEnumerable<ProjectDTO>> GetProjectsAsync(UserInfo user)
        {
            try
            {
                List<ProjectDTO> projects = await http.GetFromJsonAsync<List<ProjectDTO>>("api/projects") ?? [];

                return projects;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return [];
            }
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