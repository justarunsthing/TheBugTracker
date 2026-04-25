using TheBugTracker.Client;
using Microsoft.AspNetCore.Mvc;
using TheBugTracker.Client.Models;
using TheBugTracker.Client.Helpers;
using TheBugTracker.Client.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TheBugTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController(IProjectDTOService projectService) : ControllerBase
    {
        UserInfo UserInfo => UserInfoHelper.GetUserInfo(User)!;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects()
        {
            var projects = await projectService.GetProjectsAsync(UserInfo);

            return Ok(projects);
        }

        [HttpGet("{projectId:int}")]
        public async Task<ActionResult<ProjectDTO>> GetProjectById([FromRoute] int projectId)
        {
            ProjectDTO? project = await projectService.GetProjectByIdAsync(projectId, UserInfo);

            if (project is null)
            {
                return NotFound();
            }

            return Ok(project);
        }
    }
}