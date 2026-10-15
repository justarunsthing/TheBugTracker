using TheBugTracker.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using TheBugTracker.Client.Enums;
using TheBugTracker.Client.Models;
using TheBugTracker.Client.Helpers;
using TheBugTracker.Client.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;

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

        /// <summary>
        /// Send Invite
        /// </summary>
        /// <param name="inviteId">The ID of the invite to send</param>
        /// <remarks>
        /// Sends a previously created invite email to that invite's InviteeEmail.
        /// Only admins may send invites.
        /// </remarks>
        [HttpPost("{inviteId:int}/send")]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> SendInviteAsync([FromRoute] int inviteId)
        {
            Uri requestUri = new Uri(Request.GetEncodedUrl());
            Uri baseUri = new Uri(requestUri.GetLeftPart(UriPartial.Authority));

            bool sent = await inviteService.SendInviteAsync(baseUri, inviteId, UserInfo);

            return sent ? NoContent() : BadRequest();
        }

        /// <summary>
        /// Cancel Invite
        /// </summary>
        /// <param name="inviteId">The ID of the invite to cancel</param>
        /// <remarks>
        /// Marks an invite as invalid, only admins may cancel pending invitations and only pending invitations may be cancelled.
        /// </remarks>
        [HttpDelete("{inviteId:int}")]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> CancelInviteAsync([FromRoute] int inviteId)
        {
            await inviteService.CancelInviteAsync(inviteId, UserInfo);

            return NoContent();
        }
    }
}