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
            var response = await http.PostAsJsonAsync("api/invites", dto);
            response.EnsureSuccessStatusCode();

            InviteDTO createdInvite = await response.Content.ReadFromJsonAsync<InviteDTO>() 
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse);

            return createdInvite;
        }

        public async Task<bool> SendInviteAsync(Uri baseUri, int inviteId, UserInfo userInfo)
        {
            try
            {
                var response = await http.PostAsync($"api/invites/{inviteId}/send", null);

                return response.IsSuccessStatusCode;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public Task CancelInviteAsync(int inviteId, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}