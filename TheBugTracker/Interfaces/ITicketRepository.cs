using TheBugTracker.Client;
using TheBugTracker.Models;

namespace TheBugTracker.Interfaces
{
    public interface ITicketRepository
    {
        /// <summary>
        /// Retrives all open tickets in the user's company
        /// </summary>
        /// <param name="user">The current user's claims</param>
        Task<IEnumerable<Ticket>> GetOpenTicketsAsync(UserInfo user);
    }
}