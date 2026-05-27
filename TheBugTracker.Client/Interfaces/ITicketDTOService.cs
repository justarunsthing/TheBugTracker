using TheBugTracker.Client.Models;

namespace TheBugTracker.Client.Interfaces
{
    public interface ITicketDTOService
    {
        /// <summary>
        /// Retrives all open tickets in the user's company
        /// </summary>
        /// <param name="user">The current user's claims</param>
        Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo user);
    }
}