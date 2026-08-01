using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using TheBugTracker.Client;
using TheBugTracker.Client.Helpers;
using TheBugTracker.Data;
using TheBugTracker.Models;

namespace TheBugTracker.Controllers
{
    [Route("uploads")]
    [ApiController]
    public class UploadsController(ApplicationDbContext context) : ControllerBase
    {
        [SwaggerIgnore]
        [HttpGet("{id:guid}")]
        [OutputCache(VaryByRouteValueNames = ["id"], Duration = 60 * 60 * 24)]
        public async Task<IActionResult> GetImageAsync(Guid id)
        {
            var image = await context.Uploads.FirstOrDefaultAsync(i => i.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            return File(image.Data!, image.Type!);
        }

        // New endpoint for /api/attachments
        [HttpGet("/api/attachments/{uploadId:guid}")]
        [Authorize]
        public async Task<ActionResult> DownloadAttachmentAsync([FromRoute] Guid uploadId)
        {
            UserInfo userInfo = UserInfoHelper.GetUserInfo(User)!;
            TicketAttachment? attachment = await context.Attachments
                .Include(a => a.Upload)
                .FirstOrDefaultAsync(a => a.UploadId == uploadId && a.Ticket!.Project!.CompanyId == userInfo.CompanyId);

            if (attachment is null)
            {
                return NotFound();
            }

            // Set the Content-Disposition header to indicate that the file should be downloaded as an attachment
            Response.Headers.TryAdd("Content-Disposition", $"attachment; filename={attachment.FileName}");

            return File(attachment.Upload!.Data!, attachment.Upload!.Type!, attachment.FileName);
        }
    }
}