using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VueBugTrackerProject.Classes;
using Microsoft.EntityFrameworkCore;

namespace VueBugTrackerProject.Server.Controllers
{
    /// <summary>
    /// Controller for viewing and managing projects.
    /// </summary>
    [ApiController]
    [Route("/projects")]
    public class ProjectController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;
        private readonly UserManager<Account> _userManager;

        public ProjectController(DatabaseContext context, UserManager<Account> userManager)
        {
            _dbContext = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Adds a new project.
        /// </summary>
        /// <param name="projectDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDTO projectDTO)
        {
            try
            {
                //Ensures user is logged in
                var account = await _userManager.GetUserAsync(User);
                if (account == null) return BadRequest("User does not exist");

                //Creates new project
                var project = new Project
                {
                    Name = projectDTO.Name,
                    Summary = projectDTO.Summary,
                    Link = projectDTO.Link,
                    Visibility = projectDTO.Visibility,
                    Description = projectDTO.Description,
                    FormattedDescription = projectDTO.FormattedDescription,
                    Tags = projectDTO.Tags,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    Owner = account
                };

                //Adds project to database
                await _dbContext.Projects.AddAsync(project);
                await _dbContext.SaveChangesAsync();

                return Created($"project/{project.ID}",projectDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //TODO:
        //Add filtering, pagination
        /// <summary>
        /// Gets a list of projects in the database.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("get")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProjects()
        {
            try
            {

                var projectPreviews = new List<ProjectPreviewViewModel>();

                //Gets all projects, with their owners and lists of bugs
                var projects = await _dbContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Bugs)
                    .ToListAsync();

                //If user is not logged in, only return projects that can be seen by all
                if (!User.Identity.IsAuthenticated)
                    projects = projects.Where(p => p.Visibility == Visibility.Public).ToList() ;
                else
                {
                    //If user is logged in, return projects that are public, available to logged in users only
                    //or are hidden and the user is allowed to view or edit them
                    var account = await _userManager.GetUserAsync(User);
                    projects = projects.Where(p => p.Visibility != Visibility.Restricted || p.UserPermissions.Any(up => up.Account == account)).ToList();
                }

                //Creates view models based on projects found
                foreach (var project in projects)
                {
                    projectPreviews.Add(new ProjectPreviewViewModel(project));
                }
                //Returns the projects
                return Ok(projectPreviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        /// <summary>
        /// Gets a project from the database.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get/{projectId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProject(string projectId)
        {
            try
            {
                var project = await _dbContext.Projects.FindAsync(projectId);
                if (project == null) return NotFound("Project does not exist");

                if (project.Visibility == Visibility.Restricted)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (project.Owner != currentUser || !project.UserPermissions.Any(up => up.Account == currentUser))
                        return Forbid("Permission denied");
                }

                if (project.Visibility == Visibility.LoggedInOnly)
                {
                    if (!User.Identity.IsAuthenticated) return Forbid("Permission denied");
                }

                //TODO: add project view model
                return Ok(project.Name);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a project.
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete")]
        [Authorize]
        public async Task<IActionResult> DeleteProject([FromBody] string projectID)
        {
            try
            {
                //Ensures user is logged in
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null) return BadRequest();

                //Looks for project by ID
                var projectToDelete = await _dbContext.Projects.FindAsync(projectID);

                //Bounces if project does not exist with ID given
                if (projectToDelete == null) return Forbid("Invalid delete operation");

                //Bounces if user does not own project
                if (projectToDelete.Owner != currentUser) return Forbid("Invalid delete operation");

                //Removes project from database
                _dbContext.Projects.Remove(projectToDelete);
                await _dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

