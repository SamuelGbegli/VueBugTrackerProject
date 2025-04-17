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
        [HttpGet]
        [Route("get")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProjects([FromQuery] int pageNumber = 1)
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

                var projectContainer = new ProjectContainer { TotalProjects = projects.Count};

                //Creates view models based on projects found
                foreach (var project in projects.OrderByDescending(p => p.DateModified).Skip(30 * (pageNumber - 1)).Take(30))
                {
                    projectContainer.Projects.Add(new ProjectPreviewViewModel(project));
                }

                //Returns the projects
                    return Ok(projectContainer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        /// <summary>
        /// Returns the number of projects the user can see.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getprojectcount")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNumberOfProjects()
        {
            try
            {
                //Gets all projects
                var projects = await _dbContext.Projects.ToListAsync();

                //If user is not logged in, only return projects that can be seen by all
                if (!User.Identity.IsAuthenticated)
                    projects = projects.Where(p => p.Visibility == Visibility.Public).ToList();
                else
                {
                    //If user is logged in, return projects that are public, available to logged in users only
                    //or are hidden and the user is allowed to view or edit them
                    var account = await _userManager.GetUserAsync(User);
                    projects = projects.Where(p => p.Visibility != Visibility.Restricted || p.UserPermissions.Any(up => up.Account == account)).ToList();
                }

                return Ok(projects.Count);

            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Returns the 5 most recently edited projects the user can vies.
        /// </summary>
        /// <param name="getUserProjects">If true, only gets projects the user made</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getrecentprojects")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecentProjects([FromQuery] bool getUserProjects)
        {
            try
            {
                var projectPreviews = new List<ProjectPreviewViewModel>();

                //Gets all projects, in order of when they were created with their owners and lists of bugs
                var projects = await _dbContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Bugs)
                    .OrderByDescending(p => p.DateModified)
                    .ToListAsync();

                if (getUserProjects)
                {
                    //Bounces the user if they are not logged in
                    if(!User.Identity.IsAuthenticated) return Unauthorized();

                    //Gets thw user's account
                    var account = await _userManager.GetUserAsync(User);

                    //Gets projects that the user owns
                    projects = projects.Where(p => p.Owner == account).ToList();
                }
                else
                {
                    //If user is not logged in, only return projects that can be seen by all
                    if (!User.Identity.IsAuthenticated)
                        projects = projects.Where(p => p.Visibility == Visibility.Public).ToList();
                    else
                    {
                        //If user is logged in, return projects that are public, available to logged in users only
                        //or are hidden and the user is allowed to view or edit them
                        var account = await _userManager.GetUserAsync(User);
                        projects = projects.Where(p => p.Visibility != Visibility.Restricted || p.UserPermissions.Any(up => up.Account == account)).ToList();
                    }
                }

                    //Creates view models based on the first 5 projects found
                    foreach (var project in projects.Take(5))
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
                //Looks for project by ID
                var project = await _dbContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Bugs)
                    .FirstOrDefaultAsync(p => p.ID == projectId);

                //Returns error if project does not exist
                if (project == null) return NotFound("Project does not exist");

                //For restricted projects, checks if user has been granted permission to view it
                if (project.Visibility == Visibility.Restricted)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser == null) return Unauthorized("Restricted project");

                    //Bounces user if they do not own the project or have permission to view it
                    if (project.Owner != currentUser && !project.UserPermissions.Any(up => up.Account == currentUser))
                        return Forbid();
                }

                //For projects only visible to logged in users, checks if user is logged in
                if (project.Visibility == Visibility.LoggedInOnly)
                {
                    if (!User.Identity.IsAuthenticated) return Unauthorized("Login reqired");
                }

                //Returns project view model
                return Ok(new ProjectViewModel(project));
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
                if (projectToDelete == null) return NotFound("Project does not exist");

                //Bounces if user does not own project
                if (projectToDelete.Owner != currentUser) return Forbid();

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

        /// <summary>
        /// Function to create dummy projects for testing.  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("createtestprojects")]
        [Authorize]
        public async Task<IActionResult> CreateTestProjects()
        {
            try
            {
                var account = await _userManager.GetUserAsync(User);
                for (int i = 1; i <= 4; i++)
                {
                    //Creates new project
                    var project = new Project
                    {
                        Name = $"Test project {i}",
                          Summary = $"Test project {i}",
                        Visibility = Visibility.Public,
                        DateCreated = DateTime.UtcNow,
                        DateModified = DateTime.UtcNow,
                        Owner = account
                    };

                    for (int j = 1; j <= 25 * i; j++)
                    {
                        //Adds dummy bug to project for testing
                        project.Bugs.Add(new Bug
                        {
                            Summary = $"Test bug {j}",
                            Severity = Severity.Low,
                            Status = Status.Open,
                            Description = $"This is dummy bug {j} for test job {i}",
                            Creator = account,
                            DateCreated = DateTime.UtcNow,
                            DateModified = DateTime.UtcNow,
                        });
                    }
                    await _dbContext.Projects.AddAsync(project);
                }

                await _dbContext.SaveChangesAsync();
                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

