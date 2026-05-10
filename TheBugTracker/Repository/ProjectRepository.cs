using TheBugTracker.Data;
using TheBugTracker.Models;
using TheBugTracker.Client;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TheBugTracker.Repository
{
    public class ProjectRepository(IDbContextFactory<ApplicationDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IProjectRepository
    {
        public async Task<Project?> GetProjectByIdAsync(int projectId, UserInfo user)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            Project? project = await context.Projects
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.SubmitterUser)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.DeveloperUser)
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId);

            return project;
        }

        public async Task<IEnumerable<Project>> GetProjectsAsync(UserInfo user)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            IEnumerable<Project> projects = await context.Projects
                .Where(p => p.CompanyId == user.CompanyId && !p.IsArchived)
                .ToListAsync();

            return projects;
        }

        public async Task<IEnumerable<ApplicationUser>> GetProjectMembersAsync(int projectId, UserInfo user)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            List<ApplicationUser> members = await context.Projects
                .Where(p => p.Id == projectId && p.CompanyId == user.CompanyId)
                .SelectMany(p => p.Members)
                .ToListAsync();

            return members;
        }

        public async Task<Project> CreateProjectAsync(Project project, UserInfo user)
        {
            bool isAdmin = user.Roles.Any(r => r == nameof(Role.Admin));
            bool isPm = user.Roles.Any(r => r == nameof(Role.ProjectManager));

            if (!isAdmin && !isPm)
            {
                throw new ApplicationException($"User {user.Email} is not authorized to create a project because they are not an Admin or a Project Manager");
            }

            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            project.CompanyId = user.CompanyId;
            project.Created = DateTimeOffset.UtcNow;

            if (isPm)
            {
                ApplicationUser projectManager = await context.Users.FirstAsync(u => u.Id == user.UserId);

                project.Members.Add(projectManager);
            }

            context.Add(project);
            await context.SaveChangesAsync();

            return project;
        }

        public async Task UpdateProjectAsync(Project project, UserInfo user)
        {
            bool canEditProject = await UserCanEditProject(project.Id, user);

            if (canEditProject)
            {
                await using ApplicationDbContext context = contextFactory.CreateDbContext();

                // Clear out navigation properties to avoid accidentally overwriting related data, i.e no tracking of related entities
                project.Members = [];
                project.Tickets = [];

                context.Projects.Update(project);
                await context.SaveChangesAsync();
            }
        }

        public async Task ArchiveProjectAsync(int projectId, UserInfo user)
        {
            bool canEditProject = await UserCanEditProject(projectId, user);

            if (!canEditProject)
            {
                return;
            }

            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            Project project = await context.Projects
                .Include(p => p.Tickets)
                .FirstAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId);

            project.IsArchived = true;

            foreach (Ticket ticket in project.Tickets)
            {
                // If ticket.IsArchived == true, then the ticket was archived by a user
                // If ticket.IsArchived == false, then the ticket will be archived by the project
                ticket.IsArchivedByProject = !ticket.IsArchived;
                ticket.IsArchived = true;
            }

            await context.SaveChangesAsync();
        }

        public async Task RestoreProjectAsync(int projectId, UserInfo user)
        {
            bool canEditProject = await UserCanEditProject(projectId, user);

            if (!canEditProject)
            {
                return;
            }

            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            Project project = await context.Projects
                .Include(p => p.Tickets)
                .FirstAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId);

            project.IsArchived = false;

            foreach (Ticket ticket in project.Tickets)
            {
                // If ticket.IsArchivedByProject == true, then the ticket should no longer be archived
                // If ticket.IsArchivedByProject == false, then the ticket was archived by a user and should remain archived
                ticket.IsArchived = !ticket.IsArchivedByProject;
                ticket.IsArchivedByProject = false;
            }

            await context.SaveChangesAsync();
        }

        public async Task AddProjectMemberAsync(int projectId, string userId, UserInfo user)
        {
            bool canEditProject = await UserCanEditProject(projectId, user);

            if (!canEditProject)
            {
                return;
            }

            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            Project project = await context.Projects
                .Include(p => p.Members)
                .FirstAsync(p => p.Id == projectId);

            if (project.Members.Any(m => m.Id == userId))
            {
                return;
            }

            ApplicationUser? newMember = await context.Users
                .Where(u => u.CompanyId == user.CompanyId)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (newMember is null 
                || await userManager.IsInRoleAsync(newMember, nameof(Role.ProjectManager)) 
                || await userManager.IsInRoleAsync(newMember, nameof(Role.Admin)))
            {
                return;
            }

            project.Members.Add(newMember);

            await context.SaveChangesAsync();
        }

        public async Task RemoveProjectMemberAsync(int projectId, string userId, UserInfo user)
        {
            bool canEditProject = await UserCanEditProject(projectId, user);

            if (!canEditProject)
            {
                return;
            }

            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            Project project = await context.Projects
               .Include(p => p.Members)
               .FirstAsync(p => p.Id == projectId);

            ApplicationUser? member = project.Members.FirstOrDefault(m => m.Id == userId);

            if (member is null
                || await userManager.IsInRoleAsync(member, nameof(Role.ProjectManager)))
            {
                return;
            }

            project.Members.Remove(member);

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Checks that the project exists and belongs to the user's company, then checks that the user is either an admin or the project manager assigned to the project
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="user"></param>
        private async Task<bool> UserCanEditProject(int projectId, UserInfo user)
        {
            bool isAdmin = user.Roles.Any(r => r == nameof(Role.Admin));
            bool isPm = user.Roles.Any(r => r == nameof(Role.ProjectManager));

            if (!isAdmin && !isPm)
            {
                return false;
            }

            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            bool canEditProject = await context.Projects
                // Project must exist and belong to the user's company
                .Where(p => p.Id == projectId && p.CompanyId == user.CompanyId)
                // User must be an admin or the project manager assigned to the project
                .AnyAsync(p => isAdmin || p.Members.Any(m => m.Id == user.UserId));

            return canEditProject;
        }

        public Task<ApplicationUser?> GetProjectManagerAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task AssignProjectManagerAsync(int projectId, string managerId, UserInfo user)
        {
            throw new NotImplementedException();
        }

        public Task RemoveProjectManagerAsync(int projectId, UserInfo user)
        {
            throw new NotImplementedException();
        }
    }
}