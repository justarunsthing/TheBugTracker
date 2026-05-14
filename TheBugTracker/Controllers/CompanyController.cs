using TheBugTracker.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TheBugTracker.Client.Enums;
using TheBugTracker.Client.Models;
using TheBugTracker.Client.Helpers;
using TheBugTracker.Client.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TheBugTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyController(ICompanyDTOService companyService) : ControllerBase
    {
        private UserInfo UserInfo => UserInfoHelper.GetUserInfo(User)!;

        /// <summary>
        /// Get Users
        /// </summary>
        /// <param name="role">
        /// Optionally filters users to a specific role
        /// </param>
        /// <remarks>
        /// Returns a collection of members of the current user's company.
        /// If a role is supplied, only returns members in that role.
        /// </remarks>

        [HttpGet("users")] // api/company/users
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers([FromQuery] Role? role)
        {
            if (role.HasValue)
            {
                IEnumerable<UserDTO> usersInRole = await companyService.GetUsersInRoleAsync(role.Value, UserInfo);

                return Ok(usersInRole);
            }
            else
            {
                IEnumerable<UserDTO> users = await companyService.GetUsersAsync(UserInfo);

                return Ok(users);
            }
        }
    }
}