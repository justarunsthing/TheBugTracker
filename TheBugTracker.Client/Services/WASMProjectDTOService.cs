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

        public async Task<IEnumerable<ProjectDTO>> GetArchivedProjectsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProjectDTO>> GetAssignedProjectsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a list of users currently assigned to the project
        /// </summary>
        /// <param name="projectId">The Id of the project</param>
        /// <param name="user">The current user's claims</param>
        /// <returns>A collection of users</returns>
        public async Task<IEnumerable<UserDTO>> GetProjectMembersAsync(int projectId, UserInfo user)
        {
            try
            {
                List<UserDTO> members = await http.GetFromJsonAsync<List<UserDTO>>($"api/projects/members/{projectId}") ?? [];

                return members;
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

        public async Task AddProjectMemberAsync(int projectId, string userId, UserInfo user)
        {
            var response = await http.PutAsync($"api/projects/members/{projectId}/{userId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveProjectMemberAsync(int projectId, string userId, UserInfo user)
        {
            var response = await http.DeleteAsync($"api/projects/members/{projectId}/{userId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<UserDTO?> GetProjectManagerAsync(int projectId, UserInfo user)
        {
            try
            {
                UserDTO? projectManager = await http.GetFromJsonAsync<UserDTO>($"api/projects/manager/{projectId}");

                return projectManager;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return null;
            }
        }

        public async Task AssignProjectManagerAsync(int projectId, string managerId, UserInfo user)
        {
            var response = await http.PutAsync($"api/projects/manager/{projectId}/{managerId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveProjectManagerAsync(int projectId, UserInfo user)
        {
            var response = await http.DeleteAsync($"api/projects/manager/{projectId}");
            response.EnsureSuccessStatusCode();
        }
    }
}