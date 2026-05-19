using TheBugTracker.Client;
using Microsoft.AspNetCore.Mvc;
using TheBugTracker.Client.Enums;
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

        /// <summary>
        /// Get Project Members
        /// </summary>
        /// <param name="projectId">The id of the project</param>
        /// <remarks>
        /// Returns all members assigned to the project
        /// </remarks>
        [HttpGet("members/{projectId:int}")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetProjectMembers([FromRoute] int projectId)
        {
            IEnumerable<UserDTO> members = await projectService.GetProjectMembersAsync(projectId, UserInfo);

            return Ok(members);
        }

        /// <summary>
        /// Get Project Manager
        /// </summary>
        /// <param name="projectId">The id of the project</param>
        /// <remarks>
        /// Returns the manager assigned to the project or null if no manager is assigned
        /// </remarks>
        [HttpGet("manager/{projectId:int}")]
        public async Task<ActionResult<UserDTO>> GetProjectManager([FromRoute] int projectId)
        {
            UserDTO? projectManager = await projectService.GetProjectManagerAsync(projectId, UserInfo);

            if (projectManager is null)
            {
                return NotFound();
            }

            return Ok(projectManager);
        }

        /// <summary>
        /// Create project
        /// </summary>
        /// <remarks>
        /// Creates a new project for the user's company
        /// Users must be a project manager or an admin to create a new project.
        /// If the user is a project manager, they will be assignd to the submitted project.
        /// </remarks>
        /// <param name="project">The details of the project to be created</param>
        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<ActionResult<ProjectDTO>> CreateProject([FromBody] ProjectDTO project)
        {
            ProjectDTO? createdProject = await projectService.CreateProjectAsync(project, UserInfo);

            return CreatedAtAction(
                actionName: nameof(GetProjectById), 
                routeValues: new { projectId = createdProject.Id }, 
                value: createdProject
            );
        }

        /// <summary>
        /// Update project
        /// </summary>
        /// <remarks>
        /// Updates the details of a specific project if it exists.
        /// Users must be an admin or the project manager assigned to the project to submit an update
        /// </remarks>
        /// <param name="projectId">The Id of the project to update</param>
        /// <param name="project">The updated details for this project</param>
        [HttpPut("{projectId:int}")] // api/projects/8
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<IActionResult> UpdateProject([FromRoute] int projectId, [FromBody] ProjectDTO project)
        {
            if (projectId != project.Id)
            {
                return BadRequest();
            }

            await projectService.UpdateProjectAsync(project, UserInfo!);

            return NoContent();
        }

        /// <summary>
        /// Archive Project
        /// </summary>
        /// <remarks>
        /// Archive a project to indicate it is no longer being worked on
        /// Users must be an admin or the project manager assigned to the project to archive the project
        /// </remarks>
        /// <param name="projectId">The Id of the project to archive</param>
        [HttpPatch("archive/{projectId:int}")]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<IActionResult> ArchiveProject([FromRoute] int projectId)
        {
            await projectService.ArchiveProjectAsync(projectId, UserInfo);

            return NoContent();
        }

        /// <summary>
        /// Restore Project
        /// </summary>
        /// <remarks>
        /// Restore a project to indicate it is active and work is resumed
        /// Users must be an admin or the project manager assigned to the project to resotre the project
        /// </remarks>
        /// <param name="projectId">The Id of the project to restore</param>
        [HttpPatch("restore/{projectId:int}")]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<IActionResult> RestoreProject([FromRoute] int projectId)
        {
            await projectService.RestoreProjectAsync(projectId, UserInfo);

            return NoContent();
        }

        /// <summary>
        /// Add Project Member
        /// </summary>
        /// <param name="projectId">The Id of the project</param>
        /// <param name="userId">The Id of the user</param>
        /// <remarks>
        /// Assigns a user to a project, if they are not already assigned
        /// *Note: Project managers must be assigned by the AssignProjectManager endpoint. Admins cannot be assigned to projects.*
        /// *Only Admins and the assigned project manager may add members to a project.*
        /// </remarks>
        [HttpPut("members/{projectId:int}/{userId}")]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<IActionResult> AddProjectMember([FromRoute] int projectId, [FromRoute] string userId)
        {
            await projectService.AddProjectMemberAsync(projectId, userId, UserInfo);

            return NoContent();
        }

        /// <summary>
        /// Assign Project Manager
        /// </summary>
        /// <param name="projectId">The id of the project</param>
        /// <param name="userId">The id of the user</param>
        /// <remarks>
        /// Assigns a project manager to a project. If another project manager is currently assigned, 
        /// they will be removed and replaced with the new project manager.
        /// If the user is not a project manager, they may not be assigned to manage the project.
        /// </remarks>
        [HttpPut("manager/{projectId:int}/{userId}")]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<IActionResult> AssignProjectManager([FromRoute] int projectId, [FromRoute] string userId)
        {
            await projectService.AssignProjectManagerAsync(projectId, userId, UserInfo);

            return NoContent();
        }

        /// <summary>
        /// Remove Project Member
        /// </summary>
        /// <param name="projectId">The Id of the project</param>
        /// <param name="userId">The Id of the user</param>
        /// <remarks>
        /// Removes a user from a project, if they are currently assigned
        /// *Note: Only Admins and the assigned project manager may remove members from a project. The Project manager 
        /// must be removed using the RemoveProjectManager endpoint.*
        /// </remarks>
        [HttpDelete("members/{projectId:int}/{userId}")]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<IActionResult> RemoveProjectMember([FromRoute] int projectId, [FromRoute] string userId)
        {
            await projectService.RemoveProjectMemberAsync(projectId, userId, UserInfo);

            return NoContent();
        }

        /// <summary>
        /// Remove Project Manager
        /// </summary>
        /// <param name="projectId">The id of the project</param>
        /// <remarks>
        /// Unassigns a project's manager if one is currently assigned.
        /// </remarks>
        [HttpDelete("manager/{projectId:int}")]
        [Authorize(Roles = $"{nameof(Role.Admin)}, {nameof(Role.ProjectManager)}")]
        public async Task<IActionResult> RemoveProjectManager([FromRoute] int projectId)
        {
            await projectService.RemoveProjectManagerAsync(projectId, UserInfo);

            return NoContent();
        }
    }
}