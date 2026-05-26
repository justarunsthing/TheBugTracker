using TheBugTracker.Client.Models;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Services
{
    public class TicketDTOService(ITicketRepository repository) : ITicketDTOService
    {
        public Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserDTO user)
        {
            throw new NotImplementedException();
        }
    }
}