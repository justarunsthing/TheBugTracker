using TheBugTracker.Client.Models;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Client.Services
{
    public class WASMTicketDTOService(HttpClient http) : ITicketDTOService
    {
        public Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TicketDTO>> GetResolvedTicketsAsync(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TicketDTO>> GetArchivedTicketsAsync(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TicketDTO>> GetAssignedTicketsAsync(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task<TicketDTO?> GetTicketByIdAsync(int id, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task<TicketDTO> CreateTicketAsync(TicketDTO ticket, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task ArchiveTicketAsync(int ticketId, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task RestoreTicketAsync(int ticketId, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTicketAsync(TicketDTO ticket, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}