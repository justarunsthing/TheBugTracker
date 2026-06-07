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
    }
}