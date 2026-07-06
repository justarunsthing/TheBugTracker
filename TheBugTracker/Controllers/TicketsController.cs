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

        /// <summary>
        /// Get Ticket by Id
        /// </summary>
        /// <param name="id">The Id of the ticket to return</param>
        /// <remarks>
        /// Returns detailed information about a specific ticket.
        /// Returns 404 Not Found if the ticket does not exist.
        /// </remarks>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TicketDTO?>> GetTicketById([FromRoute] int id)
        {
            TicketDTO? ticket = await ticketService.GetTicketByIdAsync(id, UserInfo);

            if (ticket is null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }

        [HttpPost]
        public async Task<ActionResult<TicketDTO>> CreateTicket([FromBody] TicketDTO ticket)
        {
            try
            {
                var createdTicket = await ticketService.CreateTicketAsync(ticket, UserInfo);

                return CreatedAtAction(actionName: nameof(GetTicketById), routeValues: new { id = createdTicket.Id }, value: createdTicket);
            }
            catch (ApplicationException invalidProjectException)
            {
                Console.WriteLine(invalidProjectException);

                return BadRequest(invalidProjectException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return Problem();
            }
        }

        /// <summary>
        /// Update Ticket
        /// </summary>
        /// <param name="id">The Id of the ticket to update</param>
        /// <param name="ticket">The updated ticket details</param>
        /// <remarks>Updates a specific ticket if it exists and the user is authorized</remarks>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTicket([FromRoute] int id, [FromBody] TicketDTO ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest();
            }

            await ticketService.UpdateTicketAsync(ticket, UserInfo);

            return NoContent();
        }

        /// <summary>
        /// Archive Ticket
        /// </summary>
        /// <param name="id">The ID of the ticket to archive</param>
        /// <remarks>Archives a specific ticket if it exists and the user is authorized</remarks>
        [HttpPatch("archive/{id:int}")]
        public async Task<IActionResult> ArchiveTicket([FromRoute] int id)
        {
            await ticketService.ArchiveTicketAsync(id, UserInfo);

            return NoContent();
        }

        /// <summary>
        /// Restore Ticket
        /// </summary>
        /// <param name="id">The ID of the ticket to restore</param>
        /// <remarks>Un-archives a specific ticket if it exists and the user is authorized</remarks>
        [HttpPatch("restore/{id:int}")]
        public async Task<IActionResult> RestoreTicket([FromRoute] int id)
        {
            await ticketService.RestoreTicketAsync(id, UserInfo);

            return NoContent();
        }
    }
}