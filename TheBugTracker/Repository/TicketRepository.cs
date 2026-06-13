using TheBugTracker.Data;
using TheBugTracker.Client;
using TheBugTracker.Models;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TheBugTracker.Repository
{
    public class TicketRepository(IDbContextFactory<ApplicationDbContext> contextFactory, UserManager<ApplicationUser> userManager) : ITicketRepository
    {
        public async Task<IEnumerable<Ticket>> GetOpenTicketsAsync(UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            List<Ticket> tickets = await context.Tickets
                .Where(t => t.Project!.CompanyId == userInfo.CompanyId
                         && !t.IsArchived
                         && t.Status != TicketStatus.Resolved)
                .Include(t => t.Project)
                .Include(t => t.SubmitterUser)
                .Include(t => t.DeveloperUser)
                .ToListAsync();

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetResolvedTicketsAsync(UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            List<Ticket> tickets = await context.Tickets
               .Where(t => t.Project!.CompanyId == userInfo.CompanyId
                        && !t.IsArchived
                        && t.Status == TicketStatus.Resolved)
               .Include(t => t.Project)
               .Include(t => t.SubmitterUser)
               .Include(t => t.DeveloperUser)
               .ToListAsync();

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetArchivedTicketsAsync(UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            List<Ticket> tickets = await context.Tickets
                .Where(t => t.Project!.CompanyId == userInfo.CompanyId
                         && t.IsArchived)
               .Include(t => t.Project)
               .Include(t => t.SubmitterUser)
               .Include(t => t.DeveloperUser)
               .ToListAsync();

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetAssignedTicketsAsync(UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            List<Ticket> tickets = [];

            if (userInfo.IsInRole(Role.ProjectManager))
            {
                List<int> assignedProjectIds = await context.Users
                    .Where(u => u.Id == userInfo.UserId)
                    .SelectMany(u => u.Projects)
                    .Select(p => p.Id)
                    .ToListAsync();

                tickets = await context.Tickets
                    .Where(t => !t.IsArchived)
                    .Where(t => t.SubmitterUserId == userInfo.UserId 
                             || t.DeveloperUserId == userInfo.UserId
                             || assignedProjectIds.Contains(t.ProjectId))
                    .Include(t => t.Project)
                    .Include(t => t.SubmitterUser)
                    .Include(t => t.DeveloperUser)
                    .ToListAsync();
            }
            else
            {
                tickets = await context.Tickets
                    .Where(t => !t.IsArchived)
                    .Where(t => t.SubmitterUserId == userInfo.UserId 
                             || t.DeveloperUserId == userInfo.UserId)
                    .Include(t => t.Project)
                    .Include(t => t.SubmitterUser)
                    .Include(t => t.DeveloperUser)
                    .ToListAsync();
            }

            return tickets;
        }

        public async Task<Ticket?> GetTicketByIdAsync(int id, UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            Ticket? ticket = await context.Tickets
                .Include(t => t.Project)
                .Include(t => t.SubmitterUser)
                .Include(t => t.DeveloperUser)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project!.CompanyId == userInfo.CompanyId);

            return ticket;
        }

        public async Task<Ticket> CreateTicketAsync(Ticket ticket, UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            Project? project = await context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == ticket.ProjectId && p.CompanyId == userInfo.CompanyId);

            if (project is null)
            {
                throw new ApplicationException("Project does not exist");
            }

            ticket.Status = TicketStatus.New;
            ticket.Created = DateTimeOffset.UtcNow;
            ticket.SubmitterUserId = userInfo.UserId;

            ApplicationUser? developer = null;

            if (!string.IsNullOrEmpty(ticket.DeveloperUserId))
            {
                bool isManagerofProject = userInfo.IsInRole(Role.ProjectManager) 
                                          && project.Members.Any(m => m.Id == userInfo.UserId);

                if (userInfo.IsInRole(Role.Admin) || isManagerofProject)
                {
                    developer = project.Members.FirstOrDefault(m => m.Id == ticket.DeveloperUserId);

                    if (developer is not null)
                    {
                        bool isDeveloper = await userManager.IsInRoleAsync(developer, nameof(Role.Developer));

                        if (!isDeveloper)
                        {
                            developer = null;
                        }
                    }
                }
            }

            ticket.DeveloperUser = developer;
            ticket.DeveloperUserId = developer?.Id;

            context.Add(ticket);
            await context.SaveChangesAsync();

            return ticket;
        }
    }
}