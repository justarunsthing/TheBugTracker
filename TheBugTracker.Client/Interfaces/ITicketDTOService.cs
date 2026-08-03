using TheBugTracker.Client.Models;

namespace TheBugTracker.Client.Interfaces
{
    public interface ITicketDTOService
    {
        /// <summary>
        /// Retrives all open tickets in the user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<TicketDTO>> GetOpenTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Retrives all resolved tickets in the user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<TicketDTO>> GetResolvedTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Retrives all archived tickets in the user's company
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<TicketDTO>> GetArchivedTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Retrives all tickets assigned to the current user. For project managers, this will retrieve all tickets
        /// they've submitted and all tickets in their assigned projects
        /// </summary>
        /// <param name="userInfo">The current user's claims</param>
        Task<IEnumerable<TicketDTO>> GetAssignedTicketsAsync(UserInfo userInfo);

        /// <summary>
        /// Retrives a ticket by its id if it exists
        /// </summary>
        /// <param name="id">The id of the ticket to retrieve</param>
        /// <param name="userInfo">The current user's claims</param>
        /// <returns>The ticket if found, otherwise null</returns>
        Task<TicketDTO?> GetTicketByIdAsync(int id, UserInfo userInfo);

        /// <summary>
        /// Creates a new ticket in the database
        /// </summary>
        /// <param name="ticket">The details of the ticket to create</param>
        /// <param name="userInfo">The current user's claims</param>
        /// <returns>The created ticket</returns>
        Task<TicketDTO> CreateTicketAsync(TicketDTO ticket, UserInfo userInfo);

        /// <summary>
        /// Archives a ticket if the user is authorized to do so
        /// </summary>
        /// <param name="ticketId">The id of the ticket to archive</param>
        /// <param name="userInfo">The current user's claims</param>
        Task ArchiveTicketAsync(int ticketId, UserInfo userInfo);

        /// <summary>
        /// Restores a ticket if the user is authorized to do so
        /// </summary>
        /// <param name="ticketId">The id of the ticket to restore</param>
        /// <param name="userInfo">The current user's claims</param>
        Task RestoreTicketAsync(int ticketId, UserInfo userInfo);

        /// <summary>
        /// Updates a ticket in the database
        /// </summary>
        /// <param name="ticket">The updated ticket information</param>
        /// <param name="userInfo">The current user's claims</param>
        Task UpdateTicketAsync(TicketDTO ticket, UserInfo userInfo);

        /// <summary>
        /// Creates a new comment for a ticket if the user is authorized to do so
        /// </summary>
        /// <param name="comment">The details of the comment to save</param>
        /// <param name="userInfo">The current user's claims</param>
        /// <returns>The created comment</returns>
        Task<TicketCommentDTO> CreateCommentAsync(TicketCommentDTO comment, UserInfo userInfo);
        
        /// <summary>
        /// Updates a comment for a ticket if it belongs to the current user
        /// </summary>
        /// <param name="comment">The updated comment information</param>
        /// <param name="userInfo">The current user's claims</param>
        Task UpdateCommentAsync(TicketCommentDTO comment, UserInfo userInfo);

        /// <summary>
        /// Deletes a comment for a ticket if it belongs to the current user or the current user is admin of the company
        /// </summary>
        /// <param name="commentId">The id of the comment to delete</param>
        /// <param name="userInfo">The current user's claims</param>
        Task DeleteCommentAsync(int commentId, UserInfo userInfo);

        /// <summary>
        /// Creates a new ticket attachment for a ticket if the user is authorized to do so.
        /// </summary>
        /// <param name="attachment"></param>

        /// <param name="userInfo"></param>
        /// <returns></returns>

        /// <summary>
        /// Uploads a new file attachment for a ticket if the user is assigned to the ticket or
        /// is the PM or an admin.
        /// </summary>
        /// <param name="attachment">The attachment to save</param>
        /// <param name="fileData">The file data of the uploaded file</param>
        /// <param name="contentType">The content type of the uploaded file</param>
        /// <param name="userInfo">The current user's claims</param>
        /// <returns>The created attachment</returns>
        Task<TicketAttachmentDTO> CreateTicketAttachmentAsync(TicketAttachmentDTO attachment, byte[] fileData, string contentType, UserInfo userInfo);

        /// <summary>
        /// Deletes a ticket attachment if the user is authorized to do so.
        /// User must be the owner of the attachment or an admin of the company.
        /// </summary>
        /// <param name="id">The id of the attachment to delete</param>
        /// <param name="userInfo">The current user's claims</param>
        Task DeleteTicketAttachmentAsync(int id, UserInfo userInfo);
    }
}