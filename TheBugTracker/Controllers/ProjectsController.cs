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

        /// <summary>
        /// Get Projects
        /// </summary>
        /// <remarks>
        /// Get all active projects belonging to the user's company
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDTO>>> GetProjects()
        {
            var projects = await projectService.GetProjectsAsync(UserInfo);

            return Ok(projects);
        }

        /// <summary>
        /// Get Project By Id
        /// </summary>
        /// <param name="projectId">
        /// The ID of the project to retrive
        /// </param>
        /// <remarks>
        /// Get detailed information about a specific project if it exists
        /// </remarks>
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