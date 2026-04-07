using TheBugTracker.Data;
using TheBugTracker.Models;
using TheBugTracker.Client;
using TheBugTracker.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TheBugTracker.Repository
{
    public class ProjectRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : IProjectRepository
    {
        public async Task<IEnumerable<Project>> GetProjectsAsync(UserInfo user)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            IEnumerable<Project> projects = await context.Projects
                .Where(p => p.CompanyId == user.CompanyId && !p.IsArchived)
                .ToListAsync();

            return projects;
        }
    }
}