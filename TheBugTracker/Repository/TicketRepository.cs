using TheBugTracker.Data;
using TheBugTracker.Client;
using TheBugTracker.Models;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheBugTracker.Client.Extensions;

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
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .Include(t => t.Attachments)
                    .ThenInclude(a => a.User)
                .Include(t => t.History)
                    .ThenInclude(h => h.User)
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

            #region History

            TicketHistory createdEvent = new()
            {
                Created = ticket.Created,
                UserId = userInfo.UserId,
                Description = "Ticket created",
            };

            ticket.History.Add(createdEvent);

            if (ticket.DeveloperUser is not null)
            {
                TicketHistory assignedEvent = new()
                {
                    Created = ticket.Created,
                    UserId = userInfo.UserId,
                    Description = $"Ticket assigned to {ticket.DeveloperUser.FirstName} {ticket.DeveloperUser.LastName}"
                };

                ticket.History.Add(assignedEvent);
            }

            #endregion

            await context.SaveChangesAsync();

            return ticket;
        }

        public async Task ArchiveTicketAsync(int ticketId, UserInfo userInfo)
        {
            if (await UserCanEditTicket(ticketId, userInfo))
            {
                await using ApplicationDbContext context = contextFactory.CreateDbContext();

                Ticket ticket = await context.Tickets.FirstAsync(t => t.Id == ticketId);

                ticket.IsArchived = true;

                #region

                TicketHistory archivedEvent = new()
                {
                    Created = DateTimeOffset.UtcNow,
                    UserId = userInfo.UserId,
                    Description = "Ticket archived"
                };

                #endregion

                ticket.History.Add(archivedEvent);

                await context.SaveChangesAsync();
            }
        }

        public async Task RestoreTicketAsync(int ticketId, UserInfo userInfo)
        {
            if (await UserCanEditTicket(ticketId, userInfo))
            {
                await using ApplicationDbContext context = contextFactory.CreateDbContext();

                Ticket ticket = await context.Tickets.FirstAsync(t => t.Id == ticketId);

                ticket.IsArchived = false;

                #region History

                TicketHistory restoredEvent = new()
                {
                    Created = DateTimeOffset.UtcNow,
                    UserId = userInfo.UserId,
                    Description = "Ticket restored"
                };

                #endregion

                ticket.History.Add(restoredEvent);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateTicketAsync(Ticket ticket, UserInfo userInfo)
        {
            bool canEdit = await UserCanEditTicket(ticket.Id, userInfo);

            if (canEdit)
            {
                ticket.Updated = DateTimeOffset.UtcNow;

                // Clear navigational properties so EF doesn't try to update them
                ticket.Comments = [];
                ticket.Attachments = [];
                ticket.DeveloperUser = null;
                ticket.SubmitterUser = null;

                await using ApplicationDbContext context = contextFactory.CreateDbContext();

                context.Update(ticket);
                await context.SaveChangesAsync();
            }
        }

        public async Task<TicketComment?> GetCommentByIdAsync(int id, UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            TicketComment? comment = await context.Comments
                .FirstOrDefaultAsync(c => c.Id == id && c.Ticket!.Project!.CompanyId == userInfo.CompanyId);

            return comment;
        }

        public async Task<TicketComment> CreateCommentAsync(TicketComment comment, UserInfo userInfo)
        {
            bool canEdit = await UserCanEditTicket(comment.TicketId, userInfo);

            if (!canEdit)
            {
                throw new ApplicationException($"User {userInfo.Email} is not authorized to comment on ticket {comment.TicketId}");
            }

            comment.Created = DateTimeOffset.UtcNow;
            comment.UserId = userInfo.UserId;

            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            context.Comments.Add(comment);
            await context.SaveChangesAsync();

            return comment;
        }

        public async Task UpdateCommentAsync(TicketComment comment, UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            if (await context.Comments.AnyAsync(c => c.Id == comment.Id && c.UserId == userInfo.UserId))
            {
                context.Comments.Update(comment);

                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteCommentAsync(int commentId, UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            TicketComment? comment = null;

            if (userInfo.IsInRole(Role.Admin))
            {
                comment = await context.Comments
                    .FirstOrDefaultAsync(c => c.Id == commentId && c.Ticket!.Project!.CompanyId == userInfo.CompanyId);

            }
            else
            { 
                comment = await context.Comments
                    .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userInfo.UserId);
            }

            if (comment is not null)
            {
                context.Remove(comment);
                await context.SaveChangesAsync();
            }
        }

        public async Task<TicketAttachment> CreateTicketAttachmentAsync(TicketAttachment attachment, UserInfo userInfo)
        {
            bool canUpload = await UserCanEditTicket(attachment.TicketId, userInfo);

            if (!canUpload)
            {
                throw new ApplicationException($"User {userInfo.Email} is not authorized to upload attachments to ticket {attachment.TicketId}");
            }

            attachment.Created = DateTimeOffset.UtcNow;
            attachment.UserId = userInfo.UserId;
            
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            context.Add(attachment);
            await context.SaveChangesAsync();

            return attachment;
        }

        public async Task DeleteTicketAttachmentAsync(int id, UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            TicketAttachment? attachment;

            if (userInfo.IsInRole(Role.Admin))
            {
                attachment = await context.Attachments
                    .Include(a => a.Upload)
                    .FirstOrDefaultAsync(a => a.Id == id && a.Ticket!.Project!.CompanyId == userInfo.CompanyId);
            }
            else
            {
                attachment = await context.Attachments
                    .Include(a => a.Upload)
                    .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userInfo.UserId);
            }

            if (attachment is not null)
            {
                context.Remove(attachment);
                context.Remove(attachment.Upload!);

                await context.SaveChangesAsync();
            }
        }

        private async Task<bool> UserCanEditTicket(int ticketId, UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            bool result = false;

            if (userInfo.IsInRole(Role.Admin))
            {
                result = await context.Tickets
                    .AnyAsync(t => t.Id == ticketId 
                              && t.Project!.CompanyId == userInfo.CompanyId);
            }
            else if (userInfo.IsInRole(Role.ProjectManager))
            {
                result = await context.Tickets
                    .AnyAsync(t => t.Id == ticketId 
                              && (t.Project!.Members.Any(m => m.Id == userInfo.UserId)
                              || t.SubmitterUserId == userInfo.UserId));
            }
            else
            {
                result = await context.Tickets
                    .AnyAsync(t => t.Id == ticketId
                               && (t.SubmitterUserId == userInfo.UserId || t.DeveloperUserId == userInfo.UserId));
            }

            return result;
        }

        private async Task<List<TicketHistory>> CreateTicketHistoryAsync(Ticket newTicket, UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            var oldTicket = await context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == newTicket.Id && userInfo.CompanyId == t.Project!.CompanyId);

            if (oldTicket is null)
            {
                return [];
            }

            List<TicketHistory> events = [];

            if (oldTicket.Status != newTicket.Status)
            {
                TicketHistory statusChangedEvent = new()
                {
                    UserId = userInfo.UserId,
                    Created = DateTimeOffset.UtcNow,
                    TicketId = newTicket.Id,
                    Description = $"Status changed from {oldTicket.Status.GetDisplayName()} to {newTicket.Status.GetDisplayName()}"
                };

                events.Add(statusChangedEvent);
            }

            if (oldTicket.Type != newTicket.Type)
            {
                TicketHistory typeChangedEvent = new()
                {
                    UserId = userInfo.UserId,
                    Created = DateTimeOffset.UtcNow,
                    TicketId = newTicket.Id,
                    Description = $"Type changed from {oldTicket.Type.GetDisplayName()} to {newTicket.Type.GetDisplayName()}"
                };

                events.Add(typeChangedEvent);
            }

            if (oldTicket.Priority != newTicket.Priority)
            {
                TicketHistory priorityChangedEvent = new()
                {
                    UserId = userInfo.UserId,
                    Created = DateTimeOffset.UtcNow,
                    TicketId = newTicket.Id,
                    Description = $"Priority changed from {oldTicket.Priority.GetDisplayName()} to {newTicket.Priority.GetDisplayName()}"
                };

                events.Add(priorityChangedEvent);
            }

            if (oldTicket.Description != newTicket.Description || oldTicket.Title != newTicket.Title)
            {
                TicketHistory descriptionChangedEvent = new()
                {
                    UserId = userInfo.UserId,
                    Created = DateTimeOffset.UtcNow,
                    TicketId = newTicket.Id,
                    Description = "Title/description updated"
                };

                events.Add(descriptionChangedEvent);
            }

            if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
            {
                TicketHistory developerChangedEvent = new()
                {
                    UserId = userInfo.UserId,
                    Created = DateTimeOffset.UtcNow,
                    TicketId = newTicket.Id,
                };

                if (string.IsNullOrEmpty(newTicket.DeveloperUserId))
                {
                    developerChangedEvent.Description = "Ticket unassigned";
                }
                else
                {
                    string? developerName = await context.Users
                        .Where(u => u.Id == newTicket.DeveloperUserId && u.CompanyId == userInfo.CompanyId)
                        .Select(u => $"{u.FirstName} {u.LastName}")
                        .FirstOrDefaultAsync();

                    developerChangedEvent.Description = $"Ticket assigned to {developerName ?? "a new developer"}";
                }

                events.Add(developerChangedEvent);
            }

            return events;
        }
    }
}