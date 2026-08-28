using System.Net.Http.Json;
using TheBugTracker.Client.Enums;
using TheBugTracker.Client.Models;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Client.Services
{
    public class WASMCompanyDTOService(HttpClient http) : ICompanyDTOService
    {
        public async Task<IEnumerable<UserDTO>> GetUsersAsync(UserInfo userInfo)
        {
            try
            {
                List<UserDTO> users = await http.GetFromJsonAsync<List<UserDTO>>("api/company/users") ?? [];

                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }

        public async Task<IEnumerable<UserDTO>> GetUsersInRoleAsync(Role role, UserInfo userInfo)
        {
            try
            {
                List<UserDTO> users = await http.GetFromJsonAsync<List<UserDTO>>($"api/company/users?role={role}") ?? [];

                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }

        public Task<CompanyDTO> GetCompanyAsync(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCompanyAsync(CompanyDTO company, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public async Task AssignUserRoleAsync(string userId, Role newRole, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}