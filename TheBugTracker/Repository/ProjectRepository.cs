using TheBugTracker.Data;
using TheBugTracker.Models;
using TheBugTracker.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TheBugTracker.Repository
{
    public class ProjectRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : IProjectRepository
    {
        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            IEnumerable<Project> projects = await context.Projects.ToListAsync();

            return projects;
        }
    }
}