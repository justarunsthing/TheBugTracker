using System.Net.Http.Json;
using TheBugTracker.Client.Models;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Client.Services
{
    public class WASMInviteDTOService(HttpClient http) : IInviteDTOService
    {
        public Task<IEnumerable<InviteDTO>> GetInvitesAsync(UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public async Task<InviteDTO> CreateInviteAsync(InviteDTO dto, UserInfo userInfo)
        {
            var response = await http.PostAsJsonAsync("api/invite", dto);
            response.EnsureSuccessStatusCode();

            InviteDTO createdInvite = await response.Content.ReadFromJsonAsync<InviteDTO>() 
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse);

            return createdInvite;
        }

        public Task<bool> SendInviteAsync(Uri baseUri, int inviteId, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public Task CancelInviteAsync(int inviteId, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}