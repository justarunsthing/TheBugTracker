using TheBugTracker.Client;
using TheBugTracker.Models;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Enums;
using TheBugTracker.Client.Models;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Services
{
    public class TicketDTOService(ITicketRepository repository) : ITicketDTOService
    {
        public async Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo userInfo)
        {
            IEnumerable<Ticket> tickets = await repository.GetOpenTicketsAsync(userInfo);
            IEnumerable<TicketDTO> dtos = tickets.Select(t => t.ToDTO());

            return dtos;
        }

        public async Task<IEnumerable<TicketDTO>> GetResolvedTicketsAsync(UserInfo userInfo)
        {
            IEnumerable<Ticket> tickets = await repository.GetResolvedTicketsAsync(userInfo);
            IEnumerable<TicketDTO> dtos = tickets.Select(t => t.ToDTO());

            return dtos;
        }

        public async Task<IEnumerable<TicketDTO>> GetArchivedTicketsAsync(UserInfo userInfo)
        {
            IEnumerable<Ticket> tickets = await repository.GetArchivedTicketsAsync(userInfo);
            IEnumerable<TicketDTO> dtos = tickets.Select(t => t.ToDTO());

            return dtos;
        }

        public async Task<IEnumerable<TicketDTO>> GetAssignedTicketsAsync(UserInfo userInfo)
        {
            IEnumerable<Ticket> tickets = await repository.GetAssignedTicketsAsync(userInfo);
            IEnumerable<TicketDTO> dtos = tickets.Select(t => t.ToDTO());

            return dtos;
        }

        public async Task<TicketDTO> CreateTicketAsync(TicketDTO ticket, UserInfo userInfo)
        {
            Ticket dbTicket = new()
            {
                Title = ticket.Title,
                Description = ticket.Description,
                Created = DateTimeOffset.UtcNow,
                Status = TicketStatus.New,
                SubmitterUserId = userInfo.UserId,
                DeveloperUserId = ticket.DeveloperUserId,
                ProjectId = ticket.ProjectId,
                Priority = ticket.Priority,
                Type = ticket.Type
            };

            dbTicket = await repository.CreateTicketAsync(dbTicket, userInfo);

            return dbTicket.ToDTO();
        }
    }
}