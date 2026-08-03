using System.Net.Http.Json;
using TheBugTracker.Client.Enums;
using TheBugTracker.Client.Models;
using TheBugTracker.Client.Interfaces;

namespace TheBugTracker.Client.Services
{
    public class WASMTicketDTOService(HttpClient http) : ITicketDTOService
    {
        public async Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo userInfo)
        {
            try
            {
                List<TicketDTO> tickets = await http.GetFromJsonAsync<List<TicketDTO>>("api/Tickets") ?? [];

                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }

        public async Task<IEnumerable<TicketDTO>> GetResolvedTicketsAsync(UserInfo userInfo)
        {
            try
            {
                List<TicketDTO> tickets = await http.GetFromJsonAsync<List<TicketDTO>>($"api/Tickets?filter={TicketsFilter.Resolved}") ?? [];

                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }

        public async Task<IEnumerable<TicketDTO>> GetArchivedTicketsAsync(UserInfo userInfo)
        {
            try
            {
                List<TicketDTO> tickets = await http.GetFromJsonAsync<List<TicketDTO>>($"api/Tickets?filter={TicketsFilter.Archived}") ?? [];

                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }

        public async Task<IEnumerable<TicketDTO>> GetAssignedTicketsAsync(UserInfo userInfo)
        {
            try
            {
                List<TicketDTO> tickets = await http.GetFromJsonAsync<List<TicketDTO>>($"api/Tickets?filter={TicketsFilter.Assigned}") ?? [];

                return tickets;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return [];
            }
        }

        public async Task<TicketDTO?> GetTicketByIdAsync(int id, UserInfo userInfo)
        {
            try
            {
                var ticket = await http.GetFromJsonAsync<TicketDTO>($"api/Tickets/{id}");

                return ticket;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public async Task<TicketDTO> CreateTicketAsync(TicketDTO ticket, UserInfo userInfo)
        {
            var response = await http.PostAsJsonAsync("api/Tickets", ticket);
            TicketDTO createdTicket = await response.Content.ReadFromJsonAsync<TicketDTO>()
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse);

            return createdTicket;
        }

        public async Task UpdateTicketAsync(TicketDTO ticket, UserInfo userInfo)
        {
            var response = await http.PutAsJsonAsync($"api/Tickets/{ticket.Id}", ticket);
            response.EnsureSuccessStatusCode();
        }

        public async Task ArchiveTicketAsync(int ticketId, UserInfo userInfo)
        {
            var response = await http.PatchAsync($"api/Tickets/archive/{ticketId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task RestoreTicketAsync(int ticketId, UserInfo userInfo)
        {
            var response = await http.PatchAsync($"api/Tickets/restore/{ticketId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task<TicketCommentDTO> CreateCommentAsync(TicketCommentDTO comment, UserInfo userInfo)
        {
            var response = await http.PostAsJsonAsync($"api/Tickets/comments/{comment.TicketId}", comment);
            response.EnsureSuccessStatusCode();

            var createdComment = await response.Content.ReadFromJsonAsync<TicketCommentDTO>()
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse);

            return createdComment;
        }

        public async Task UpdateCommentAsync(TicketCommentDTO comment, UserInfo userInfo)
        {
            var response = await http.PutAsJsonAsync($"api/Tickets/{comment.TicketId}/comments/{comment.Id}", comment);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCommentAsync(int commentId, UserInfo userInfo)
        {
            var response = await http.DeleteAsync($"api/Tickets/comments/{commentId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<TicketAttachmentDTO> CreateTicketAttachmentAsync(TicketAttachmentDTO attachment, byte[] fileData, string contentType, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteTicketAttachmentAsync(int id, UserInfo userInfo)
        {
            throw new NotImplementedException();
        }
    }
}