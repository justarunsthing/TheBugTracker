using TheBugTracker.Client.Models;

namespace TheBugTracker.Client.Interfaces
{
    public interface ITicketDTOService
    {
        /// <summary>
        /// Retrives all open tickets in the user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Retrives all resolved tickets in the user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<TicketDTO>> GetResolvedTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Retrives all archived tickets in the user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<TicketDTO>> GetArchivedTicketsAsync(UserInfo userInfo);
    }
}