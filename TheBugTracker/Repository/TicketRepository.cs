using TheBugTracker.Data;
using TheBugTracker.Client;
using TheBugTracker.Models;
using TheBugTracker.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TheBugTracker.Repository
{
    public class TicketRepository(IDbContextFactory<ApplicationDbContext> contextFactory, UserManager<ApplicationUser> userManager) : ITicketRepository
    {
        public Task<IEnumerable<Ticket>> GetOpenTicketsAsync(UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}