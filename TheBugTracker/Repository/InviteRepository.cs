using TheBugTracker.Data;
using TheBugTracker.Client;
using TheBugTracker.Models;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace TheBugTracker.Repository
{
    public class InviteRepository : IInviteRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IEmailSender _emailSender;
        private readonly IDataProtector _protector;

        public InviteRepository(IDbContextFactory<ApplicationDbContext> contextFactory,
                                IEmailSender emailSender,
                                IDataProtectionProvider protectionProvider,
                                IConfiguration config)
        {
            _contextFactory = contextFactory;
            _emailSender = emailSender;

            string protectionPurpose = config["InviteProtectionKey"]
                ?? throw new ApplicationException("InviteProtectionKey not found in configuration!");

            _protector = protectionProvider.CreateProtector(protectionPurpose);
        }

        public async Task<IEnumerable<Invite>> GetInvitesAsync(UserInfo userInfo)
        {
            await using ApplicationDbContext context = _contextFactory.CreateDbContext();

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

            await using ApplicationDbContext context = _contextFactory.CreateDbContext();

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

        public async Task CancelInviteAsync(int inviteId, UserInfo userInfo)
        {
            if (!userInfo.IsInRole(Role.Admin))
            {
                return;
            }
            
            await using ApplicationDbContext context = _contextFactory.CreateDbContext();

            Invite? invite = await context.Invites
                .FirstOrDefaultAsync(i => i.Id == inviteId 
                                          && i.CompanyId == userInfo.CompanyId
                                          && i.IsValid == true);

            if (invite is not null)
            {
                invite.IsValid = false;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> SendInviteAsync(Uri baseUri, int inviteId, UserInfo userInfo)
        {
            if (!userInfo.IsInRole(Role.Admin))
            {
                return false;
            }

            try
            {
                await using ApplicationDbContext context = _contextFactory.CreateDbContext();

                Invite? invite = await context.Invites
                    .Include(i => i.Company)
                    .Include(i => i.Invitor)
                    .Include(i => i.Project)
                    .FirstOrDefaultAsync(i => i.Id == inviteId
                                              && i.CompanyId == userInfo.CompanyId
                                              && i.IsValid == true);

                if (invite is null || ValidateInvite(invite) == false)
                {
                    if (invite is not null)
                    {
                        invite.IsValid = false;
                        await context.SaveChangesAsync();
                    }

                    return false;
                }

                string protectedEmail = _protector.Protect(invite.InviteeEmail!);
                string protectedCompanyId = _protector.Protect(invite.CompanyId.ToString());
                string protectedToken = _protector.Protect(invite.CompanyToken.ToString());

                // https://mybugtracker.com OR https://localhost:5001
                string baseUrl = baseUri.GetLeftPart(UriPartial.Authority);
                string inviteUrl = $"{baseUrl}/Account/Register/Invite?token={protectedToken}&email={protectedEmail}&companyId={protectedCompanyId}";

                string subject = $"You have been invited to join {invite.Company!.Name} on The Bug Tracker!";
                string message = string.IsNullOrEmpty(invite.Message)
                    ? string.Empty
                    : $"""
                        <hr />
                        <p>Message from {invite.Invitor!.FirstName} {invite.Invitor.LastName}:</p>
                        <blockquote>{invite.Message}</blockquote>
                        <hr />
                       """;

                string body = $"""
                                <h1>Welcome to The Bug Tracker!</h1>
                                <p>
                                    {invite.Invitor!.FirstName} {invite.Invitor.LastName} has invited you to 
                                    join {invite.Company!.Name} on The Bug Tracker to work on the project
                                    {invite.Project!.Name}!
                                </p>
                                {message}
                                <p>
                                    To accept this invitation, pleaes <a href="{inviteUrl}">click here</a>
                                    to register a new account.
                                </p>
                                <p>
                                    <strong>Note:</strong> This invitation will expire on {invite.InviteDate.AddDays(7):d}
                                </p>
                                <small>
                                    If you are unable to click the link above, please copy & paste the following URL into
                                    your browser's address bar:
                                    <br />
                                    {inviteUrl}
                                </small>
                               """;

                await _emailSender.SendEmailAsync(invite.InviteeEmail!, subject, body);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
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