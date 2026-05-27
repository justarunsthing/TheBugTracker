using TheBugTracker.Client;
using TheBugTracker.Models;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Models;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Services
{
    public class TicketDTOService(ITicketRepository repository) : ITicketDTOService
    {
        public async Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo user)
        {
            IEnumerable<Ticket> tickets = await repository.GetOpenTicketsAsync(user);
            IEnumerable<TicketDTO> dtos = tickets.Select(t => t.ToDTO());

            return dtos;
        }
    }
}