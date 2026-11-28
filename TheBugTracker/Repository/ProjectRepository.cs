using TheBugTracker.Data;
using TheBugTracker.Models;
using TheBugTracker.Client;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace TheBugTracker.Repository
{
    public class ProjectRepository(IDbContextFactory<ApplicationDbContext> contextFactory, 
                                   UserManager<ApplicationUser> userManager,
                                   ICompanyRepository companyRepository) : IProjectRepository
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
                .Include(p => p.Members)
                .ToListAsync();

            return projects;
        }

        public async Task<IEnumerable<Project>> GetArchivedProjectsAsync(UserInfo user)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            IEnumerable<Project> projects = await context.Projects
                .Where(p => p.CompanyId == user.CompanyId && p.IsArchived)
                .Include(p => p.Members)
                .ToListAsync();

            return projects;
        }

        public async Task<IEnumerable<Project>> GetAssignedProjectsAsync(UserInfo user)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            IEnumerable<Project> projects = await context.Projects
                .Where(p => !p.IsArchived && p.CompanyId == user.CompanyId)
                .Where(p => user.IsInRole(Role.Admin) || p.Members.Any(m => m.Id == user.UserId))
                .Include(p => p.Members)
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

            #region Notifications

            // Notify company admins and project members
            var admins = await companyRepository.GetUsersInRoleAsync(Role.Admin, user);
            List<string> recipientIds = [.. admins.Select(a => a.Id)];

            recipientIds.AddRange(project.Members.Select(m => m.Id));

            foreach (string recipientId in recipientIds.Distinct())
            {
                if (recipientId != user.UserId)
                {
                    var notification = new Notification
                    {
                        Title = "New Project Created",
                        Message = $"Project '{project.Name}' has been created by {user.FullName}.",
                        Created = DateTimeOffset.UtcNow,
                        RecipientId = recipientId,
                        SenderId = user.UserId,
                        Type = NotificationType.Project,
                        Project = project
                    };

                    context.Notifications.Add(notification);
                }
            }

            #endregion

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

                #region Notifications

                // Notify PM
                var projectManager = await GetProjectManagerAsync(project.Id, user);

                if (projectManager is not null && projectManager.Id != user.UserId)
                {
                    var notification = new Notification
                    {
                        Title = "Project Updated",
                        Message = $"Project '{project.Name}' has been updated by {user.FullName}.",
                        Created = DateTimeOffset.UtcNow,
                        RecipientId = projectManager.Id,
                        SenderId = user.UserId,
                        Type = NotificationType.Project,
                        ProjectId = project.Id
                    };

                    context.Notifications.Add(notification);
                }

                #endregion

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
                .Include(p => p.Members)
                .FirstAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId);

            project.IsArchived = true;

            foreach (Ticket ticket in project.Tickets)
            {
                // If ticket.IsArchived == true, then the ticket was archived by a user
                // If ticket.IsArchived == false, then the ticket will be archived by the project
                ticket.IsArchivedByProject = !ticket.IsArchived;
                ticket.IsArchived = true;

                ticket.History.Add(new TicketHistory
                {
                    UserId = user.UserId,
                    Created = DateTimeOffset.UtcNow,
                    Description = "Project archived"
                });
            }

            #region Notifications

            // Notify project members
            foreach (ApplicationUser member in project.Members)
            {
                if (member.Id != user.UserId)
                {
                    var notification = new Notification
                    {
                        Title = "Project Archived",
                        Message = $"Project '{project.Name}' has been archived by {user.FullName}.",
                        RecipientId = member.Id,
                        SenderId = user.UserId,
                        Type = NotificationType.Project,
                        ProjectId = project.Id,
                        Created = DateTimeOffset.UtcNow
                    };

                    context.Notifications.Add(notification);
                }
            }

            #endregion

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
                .Include(p => p.Members)
                .FirstAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId);

            project.IsArchived = false;

            foreach (Ticket ticket in project.Tickets)
            {
                // If ticket.IsArchivedByProject == true, then the ticket should no longer be archived
                // If ticket.IsArchivedByProject == false, then the ticket was archived by a user and should remain archived
                ticket.IsArchived = !ticket.IsArchivedByProject;
                ticket.IsArchivedByProject = false;

                ticket.History.Add(new TicketHistory
                {
                    UserId = user.UserId,
                    Created = DateTimeOffset.UtcNow,
                    Description = "Project restored"
                });
            }

            #region Notifications

            // Notify project members
            foreach (ApplicationUser member in project.Members)
            {
                if (member.Id != user.UserId)
                {
                    var notification = new Notification
                    {
                        Title = "Project Restored",
                        Message = $"Project '{project.Name}' has been restored by {user.FullName}.",
                        RecipientId = member.Id,
                        SenderId = user.UserId,
                        Type = NotificationType.Project,
                        ProjectId = project.Id,
                        Created = DateTimeOffset.UtcNow
                    };

                    context.Notifications.Add(notification);
                }
            }

            #endregion

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

            #region Notifications

            // Notify new member
            var memberNotification = new Notification
            {
                Title = "Added to Project",
                Message = $"You have been added to project '{project.Name}' by {user.FullName}.",
                RecipientId = userId,
                SenderId = user.UserId,
                Created = DateTimeOffset.UtcNow,
                Type = NotificationType.Project,
                ProjectId = project.Id
            };

            context.Notifications.Add(memberNotification);

            // Notify project manager if admin added the member
            if (user.IsInRole(Role.Admin))
            {
                ApplicationUser? projectManager = await GetProjectManagerAsync(projectId, user);

                if (projectManager is not null)
                {
                    var pmNotification = new Notification
                    {
                        Title = "New Project Member Added",
                        Message = $"{newMember.FirstName} {newMember.LastName} has been added to your project '{project.Name}' by {user.FullName}.",
                        RecipientId = projectManager.Id,
                        SenderId = user.UserId,
                        Created = DateTimeOffset.UtcNow,
                        Type = NotificationType.Project,
                        ProjectId = project.Id
                    };

                    context.Notifications.Add(pmNotification);
                }
            }

            #endregion

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

        public async Task<ApplicationUser?> GetProjectManagerAsync(int projectId, UserInfo user)
        {
            IEnumerable<ApplicationUser> members = await GetProjectMembersAsync(projectId, user);

            foreach (var member in members)
            {
                if (await userManager.IsInRoleAsync(member, nameof(Role.ProjectManager)))
                {
                    return member;
                }
            }

            return null;
        }

        public async Task AssignProjectManagerAsync(int projectId, string managerId, UserInfo user)
        {
            bool canEditProject = await UserCanEditProject(projectId, user);

            if (!canEditProject)
            {
                return;
            }

            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            ApplicationUser? newManager = await context.Users
                .FirstOrDefaultAsync(u => u.Id == managerId && u.CompanyId == user.CompanyId);

            if (newManager == null || !await userManager.IsInRoleAsync(newManager, nameof(Role.ProjectManager)))
            {
                return;
            }

            Project project = await context.Projects
                .Include(p => p.Members)
                .FirstAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId);

            ApplicationUser? oldManager = await GetProjectManagerAsync(projectId, user);

            if (oldManager?.Id == newManager.Id)
            {
                return;
            }
            else if (oldManager is not null)
            {
                ApplicationUser userToRemove = project.Members.First(m => m.Id == oldManager.Id);
                project.Members.Remove(userToRemove);
            }

            project.Members.Add(newManager);
            await context.SaveChangesAsync();
        }

        public async Task RemoveProjectManagerAsync(int projectId, UserInfo user)
        {
            bool canEditProject = await UserCanEditProject(projectId, user);

            if (!canEditProject)
            {
                return;
            }

            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            Project project = await context.Projects
                .Include(p => p.Members)
                .FirstAsync(p => p.Id == projectId && p.CompanyId == user.CompanyId);

            foreach (var member in project.Members)
            {
                if (await userManager.IsInRoleAsync(member, nameof(Role.ProjectManager)))
                {
                    project.Members.Remove(member);
                    await context.SaveChangesAsync();

                    return;
                }
            }
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
    }
}