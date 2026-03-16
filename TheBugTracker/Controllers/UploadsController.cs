using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using TheBugTracker.Data;

namespace TheBugTracker.Controllers
{
    [Route("uploads")]
    [ApiController]
    public class UploadsController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet("{id:guid}")]
        [OutputCache(VaryByRouteValueNames = ["id"], Duration = 60 * 60 * 24)]
        public async Task<IActionResult> GetImageAsync(Guid id)
        {
            var image = await context.Images.FirstOrDefaultAsync(i => i.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            return File(image.Data!, image.Type!);
        }
    }
}