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

        public async Task<ProjectDTO> CreateProjectAsync(ProjectDTO project, UserInfo user)
        {
            var response = await http.PostAsJsonAsync("api/projects", project);
            response.EnsureSuccessStatusCode();

            ProjectDTO createdProject = await response.Content.ReadFromJsonAsync<ProjectDTO>() ?? throw new HttpIOException(HttpRequestError.InvalidResponse);

            return createdProject;
        }

        public async Task UpdateProjectAsync(ProjectDTO project, UserInfo user)
        {
            var response = await http.PutAsJsonAsync($"api/projects/{project.Id}", project);
            response.EnsureSuccessStatusCode();
        }

        public async Task ArchiveProjectAsync(int projectId, UserInfo user)
        {
            var response = await http.PatchAsync($"api/projects/archive/{projectId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RestoreProjectAsync(int projectId, UserInfo user)
        {
            var response = await http.PatchAsync($"api/projects/restore/{projectId}", null);
            response.EnsureSuccessStatusCode();
        }

        public Task AddProjectMemberAsync(int projectId, string userId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task RemoveProjectMemberAsync(int projectId, string userId, UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}