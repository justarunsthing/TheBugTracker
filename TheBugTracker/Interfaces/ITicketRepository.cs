using TheBugTracker.Client;
using TheBugTracker.Models;

namespace TheBugTracker.Interfaces
{
    public interface ITicketRepository
    {
        /// <summary>
        /// Retrives all open tickets in the user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<Ticket>> GetOpenTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Retrives all resolved tickets in the user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<Ticket>> GetResolvedTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Retrives all archived tickets in the user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<Ticket>> GetArchivedTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Retrives all tickets assigned to the current user. For project managers, this will retrieve all tickets
        /// they've submitted and all tickets in their assigned projects
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<Ticket>> GetAssignedTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Creates a new ticket in the database
        /// </summary>
        /// <param name="ticket">The details of the ticket to create</param>
        /// <param name="userInfo">The current user's claims</param>
        /// <returns>The created ticket</returns>
        Task<Ticket> CreateTicketAsync(Ticket ticket, UserInfo userInfo);
    }
}