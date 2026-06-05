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
    }
}