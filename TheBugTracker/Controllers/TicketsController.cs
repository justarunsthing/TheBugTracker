using TheBugTracker.Client;
using Microsoft.AspNetCore.Mvc;
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
    public class TicketsController(ITicketDTOService ticketService) : ControllerBase
    {
        private UserInfo UserInfo => UserInfoHelper.GetUserInfo(User)!;

        /// <summary>
        /// Get Tickets
        /// </summary>
        /// <param name="filter">
        /// Optionally filters tickets by resolved, assigned or archived tickets.
        /// By default, returns all open tickets.
        /// </param>
        /// <remarks>
        /// Returns a collection of tickets belonging to the user's company, returns all open tickets by default.
        /// The filter query parameters archived, assigned or resolved can be used.
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDTO>>> GetTickets([FromQuery] TicketsFilter filter = TicketsFilter.Open)
        {
            IEnumerable<TicketDTO> tickets = filter switch
            {
                TicketsFilter.Resolved => await ticketService.GetResolvedTicketsAsync(UserInfo),
                TicketsFilter.Assigned => await ticketService.GetAssignedTicketsAsync(UserInfo),
                TicketsFilter.Archived => await ticketService.GetArchivedTicketsAsync(UserInfo),
                _ => await ticketService.GetOpenTicketsAsync(UserInfo)
            };
            
            return Ok(tickets);
        }
    }
}