using TheBugTracker.Client.Models;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Client.Services
{
    public class WASMInviteDTOService : IInviteDTOService
    {
        public Task CancelInviteAsync(int inviteId, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task<InviteDTO> CreateInviteAsync(InviteDTO dto, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InviteDTO>> GetInvitesAsync(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendInviteAsync(Uri baseUri, int inviteId, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}