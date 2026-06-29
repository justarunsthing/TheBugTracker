using System.Net.Http.Json;
using TheBugTracker.Client.Models;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Client.Services
{
    public class WASMTicketDTOService(HttpClient http) : ITicketDTOService
    {
        public async Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo userInfo)
        {
            try
            {
                List<TicketDTO> tickets = await http.GetFromJsonAsync<List<TicketDTO>>("api/Tickets") ?? [];

                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
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