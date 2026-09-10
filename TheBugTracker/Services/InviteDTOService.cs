using TheBugTracker.Client;
using TheBugTracker.Models;
using TheBugTracker.Interfaces;
using TheBugTracker.Client.Models;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Services
{
    public class InviteDTOService(IInviteRepository repository) : IInviteDTOService
    {
        public async Task<InviteDTO> CreateInviteAsync(InviteDTO dto, UserInfo userInfo)
        {
            Invite invite = new()
            {
                InviteDate = DateTimeOffset.UtcNow,
                InviteeEmail = dto.InviteeEmail,
                InviteeFirstName = dto.InviteeFirstName,
                InviteeLastName = dto.InviteeLastName,
                Message = dto.Message,
                CompanyId = userInfo.CompanyId,
                ProjectId = dto.ProjectId!.Value,
                InvitorId = userInfo.UserId
            };

            Invite createdInvite = await repository.CreateInviteAsync(invite, userInfo);

            return createdInvite.ToDTO();
        }
    }
}