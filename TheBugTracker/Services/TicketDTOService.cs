using TheBugTracker.Client;
using TheBugTracker.Models;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Enums;
using TheBugTracker.Client.Models;
using Microsoft.AspNetCore.Identity;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Services
{
    public class TicketDTOService(ITicketRepository repository, UserManager<ApplicationUser> userManager) : ITicketDTOService
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

        public async Task<TicketDTO?> GetTicketByIdAsync(int id, UserInfo userInfo)
        {
            Ticket? ticket = await repository.GetTicketByIdAsync(id, userInfo);

            if (ticket is null)
            {
                return null;
            }

            TicketDTO dto = ticket.ToDTO();

            if (ticket.SubmitterUser is not null)
            {
                dto.SubmitterUser = await ticket.SubmitterUser.ToDTOWithRole(userManager);
            }

            if (dto.DeveloperUser is not null)
            {
                dto.DeveloperUser.Role = Role.Developer;
            }

            return dto;
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

        public Task ArchiveTicketAsync(int ticketId, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task RestoreTicketAsync(int ticketId, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}