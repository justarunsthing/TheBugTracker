using TheBugTracker.Data;
using TheBugTracker.Client;
using TheBugTracker.Models;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Enums;
using Microsoft.EntityFrameworkCore;

namespace TheBugTracker.Repository
{
    public class CompanyRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : ICompanyRepository
    {
        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync(UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            List<ApplicationUser> users = await context.Users
                .Where(u => u.CompanyId == userInfo.CompanyId)
                .ToListAsync();

            return users;
        }

        public Task<IEnumerable<ApplicationUser>> GetUsersInRoleAsync(Role role, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}
