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
    public class InvitesController(IInviteDTOService inviteService) : ControllerBase
    {
        private UserInfo UserInfo => UserInfoHelper.GetUserInfo(User)!;

        /// <summary>
        /// Create Invite
        /// </summary>
        /// <param name="invite">The details of the invite to create</param>
        /// <remarks>
        /// Creates a new invite record in the database for the user's company.
        /// Only admins may create invites.
        /// 
        /// **Note**: This endpoint only creates an invite but doesn't send the invite email. 
        /// Use the Send Invite endpoint to send the invite email.
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<ActionResult<InviteDTO>> CreateInviteAsync([FromBody] InviteDTO invite)
        {
            try
            {
                InviteDTO createdInvite = await inviteService.CreateInviteAsync(invite, UserInfo);

                return Ok(createdInvite);
            }
            catch (ApplicationException validationEx)
            {
                Console.WriteLine(validationEx);
                return BadRequest();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }
    }
}