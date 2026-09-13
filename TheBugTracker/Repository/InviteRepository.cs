using TheBugTracker.Data;
using TheBugTracker.Client;
using TheBugTracker.Models;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Enums;
using Microsoft.EntityFrameworkCore;

namespace TheBugTracker.Repository
{
    public class InviteRepository(IDbContextFactory<ApplicationDbContext> contextFactory) : IInviteRepository
    {
        public async Task<IEnumerable<Invite>> GetInvitesAsync(UserInfo userInfo)
        {
            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            IEnumerable<Invite> invites = await context.Invites
                .Where(i => i.CompanyId == userInfo.CompanyId)
                .Include(i => i.Invitor)
                .Include(i => i.Invitee)
                .Include(i => i.Project)
                .ToListAsync();

            foreach (Invite invite in invites)
            {
                invite.IsValid = ValidateInvite(invite);
            }

            // Save the updated IsValid status to the database
            await context.SaveChangesAsync();

            return invites;
        }

        public async Task<Invite> CreateInviteAsync(Invite invite, UserInfo userInfo)
        {
            if (!userInfo.IsInRole(Role.Admin))
            {
                throw new ApplicationException($"{userInfo.Email} is not authorized to create invites.");
            }

            await using ApplicationDbContext context = contextFactory.CreateDbContext();

            if (await context.Users.AnyAsync(u => u.Email!.ToUpper() == invite.InviteeEmail!.ToUpper()))
            {
                throw new ApplicationException($"The email {invite.InviteeEmail} is already registered.");
            }

            if (!await context.Projects.AnyAsync(p => p.Id == invite.ProjectId && p.CompanyId == userInfo.CompanyId))
            {
                throw new ApplicationException($"Project ID: {invite.ProjectId} does not exist in the company.");
            }

            invite.CompanyToken = Guid.NewGuid();
            invite.CompanyId = userInfo.CompanyId;
            invite.InvitorId = userInfo.UserId;
            invite.InviteDate = DateTimeOffset.UtcNow;
            invite.IsValid = true;
            invite.InviteeId = null;
            invite.JoinDate = null;

            context.Invites.Add(invite);
            await context.SaveChangesAsync();

            return invite;
        }

        private bool ValidateInvite(Invite invite)
        {
            bool isValid = invite.IsValid
                && DateTimeOffset.UtcNow < invite.InviteDate.AddDays(7)
                && invite.JoinDate is null
                && string.IsNullOrEmpty(invite.InviteeId);

            return isValid;
        }
    }
}