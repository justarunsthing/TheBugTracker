using TheBugTracker.Client.Enums;
using TheBugTracker.Client.Models;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Client.Services
{
    public class WASMCompanyDTOService : ICompanyDTOService
    {
        public Task<IEnumerable<UserDTO>> GetUsersAsync(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserDTO>> GetUsersInRoleAsync(Role role, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}