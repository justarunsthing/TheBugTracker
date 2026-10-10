using TheBugTracker.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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
    }
}