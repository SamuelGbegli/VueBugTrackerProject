using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using VueBugTrackerProject.Classes;

namespace VueBugTrackerProject.Server.Controllers
{
    /// <summary>
    /// Controller for managing bugs.
    /// </summary>
    [ApiController]
    [Route("/bugs")]
    public class BugController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly UserManager<Account> _userManager;

        public BugController(DatabaseContext databaseContext, UserManager<Account> userManager)
        {
            _databaseContext = databaseContext;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets a preview of a project's bugs.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        //TODO: Change to POST with bug filtering
        [HttpGet]
        [Route("getbugpreviews")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProjectBugPreviews([FromQuery] string projectId, [FromQuery] int page)
        {
            try
            {
                var project = await _databaseContext.Projects
                    .Include(p => p.Bugs)
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync(p => p.ID == projectId);
                if (project == null) return NotFound();

                // For restricted projects, checks if user has been granted permission to view it
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

                //Creates and returns a list of bug summaries
                var bugPreviews = new List<BugPreviewViewModel>();
                foreach (var bug in project.Bugs.OrderByDescending(b => b.DateModified).Skip((page - 1) * 20).Take(20))
                {
                    bugPreviews.Add(new BugPreviewViewModel(bug));
                }
                return Ok(bugPreviews);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("get/{bugId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBug(string bugId)
        {
            try
            {
                //Searches for bug based on ID
                var bug = await _databaseContext.Bugs
                    .Include(b => b.Project)
                    .Include(b => b.Creator)
                    .FirstOrDefaultAsync(b => b.ID == bugId);

                //Throws not found if bug does not exist
                if (bug == null) return NotFound();

                //For restricted projects, checks if user has been granted permission to view it
                if (bug.Project.Visibility == Visibility.Restricted)
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser == null) return Unauthorized("Restricted project");

                    //Bounces user if they do not own the project or have permission to view it
                    if (bug.Project.Owner != currentUser && !bug.Project.UserPermissions.Any(up => up.Account == currentUser))
                        return Forbid();
                }

                //For projects only visible to logged in users, checks if user is logged in
                if (bug.Project.Visibility == Visibility.LoggedInOnly)
                {
                    if (!User.Identity.IsAuthenticated) return Unauthorized("Login reqired");
                }

                var bugViewModel = new BugViewModel(bug);
                return Ok(new BugViewModel(bug));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Gets the number of bugs in a project.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getnumberofbugs/{projectId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProjectBugCount(string projectId)
        {
            try
            {
                //Looks for project by ID
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Bugs)
                    .FirstOrDefaultAsync(p => p.ID == projectId);
                if (project == null) return NotFound();

                //Ensures users not logged in can only see public projects
                if (!User.Identity.IsAuthenticated)
                {
                    if (project.Visibility != Visibility.Public) return Unauthorized();
                }

                //If project is restricted, checks if user owns project or can view it
                if (project.Visibility == Visibility.Restricted)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (project.Owner != user && project.UserPermissions.Any(u => u.Account == user))
                        return Forbid();
                }

                //Returns nubmer of bugs in project
                return Ok(project.Bugs.Count());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        /// Adds a bug to a project.
        /// </summary>
        /// <param name="bugDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addbug")]
        [Authorize]
        public async Task<IActionResult> AddBug([FromBody] BugDTO bugDTO)
        {
            try
            {
                //Gets user
                var account = await _userManager.GetUserAsync(User);

                //Looks for project by ID
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync(p => p.ID == bugDTO.ProjectID);
                if (project == null) return NotFound();

                //Checks if user owns the project
                //TODO: add checking for user permissions
                if (project.Owner != account) return Forbid();

                //Creates and adds bug
                var bug = new Bug
                {
                    Summary = bugDTO.Summary,
                    Description = bugDTO.Description,
                    Severity = bugDTO.Severity,
                    Status = Status.Open,
                    Creator = account,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                };

                bug.Comments.Add(new Comment
                {
                    Owner = account,
                    Text = $"Created bug on {DateTime.UtcNow}",
                   IsStatusUpdate = true,
                   DatePosted = DateTime.UtcNow                   
                });

                project.Bugs.Add(bug);
                project.DateModified = DateTime.UtcNow;
                await _databaseContext.SaveChangesAsync();

                return Created();


            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Updates a bug in the database.
        /// </summary>
        /// <param name="bugDTO"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("updatebug")]
        [Authorize]
        public async Task<IActionResult> UpdateBug([FromBody] BugDTO bugDTO)
        {
            try
            {

                //Gets user account
                var account = await _userManager.GetUserAsync(User);

                //Gets project
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Bugs)
                    .FirstOrDefaultAsync(b => b.ID == bugDTO.ProjectID);
                if (project == null) return NotFound();

                //Checks if bug exists in project
                var bug = await _databaseContext.Bugs
                    .Include(b => b.Comments)
                    .FirstOrDefaultAsync(b => b.ID == bugDTO.BugID);
                if (bug == null) return NotFound();

                //Checks if user owns the project or created the bug
                if (project.Owner != account && bug.Creator != account) return Forbid();

                //Adds status update if bug severity changes
                if(bug.Severity != bugDTO.Severity)
                    bug.Comments.Add(new Comment {
                        Owner = account,
                        Text = $"Changed bug severity to ${(bugDTO.Severity == Severity.Low ? "Low" : bugDTO.Severity == Severity.Medium ? "Medium" : "High")}",
                        IsStatusUpdate = true,
                        DatePosted = DateTime.UtcNow
                    });

                //Updates bug
                bug.Summary = bugDTO.Summary;
                bug.Description = bugDTO.Description;
                bug.Severity = bugDTO.Severity;
                bug.Status = bugDTO.IsOpen ? Status.Open : Status.Closed;
                //TODO: add bug status update message
                bug.DateModified = DateTime.UtcNow;
                project.DateModified = DateTime.UtcNow;

                //Saves changes
                await _databaseContext.SaveChangesAsync();
                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Opens or closes a bug.
        /// </summary>
        /// <param name="bugDTO"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("togglebugstatus")]
        [Authorize]
        public async Task<IActionResult> ToggleBugStatus([FromBody] string bugId)
        {
            try
            {
                //Gets user account
                var account = await _userManager.GetUserAsync(User);


                //Gets bug
                var bug = await _databaseContext.Bugs
                    .Include(b => b.Creator)
                    .Include(b => b.Project)
                    .FirstOrDefaultAsync(b => b.ID == bugId);
                if (bug == null) return NotFound();

                //Gets project
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync(p => p.ID == bug.Project.ID);
                if (project == null) return NotFound();

                //Checks if user owns the project or created the bug
                if (project.Owner != account && bug.Creator != account) return Forbid();

                //Updates bug
                bug.Status = bug.Status == Status.Closed ? Status.Open : Status.Closed;

                bug.Comments.Add(new Comment
                {
                    Owner = account,
                    Text = $"{(bug.Status == Status.Open ? "Reopened" : "Closed")} the bug.",
                    IsStatusUpdate = true,
                    DatePosted = DateTime.UtcNow
                });

                bug.DateModified = DateTime.UtcNow;
                project.DateModified = DateTime.UtcNow;

                //Saves changes
                await _databaseContext.SaveChangesAsync();
                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a bug from a project.
        /// </summary>
        /// <param name="bugDTO"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deletebug")]
        [Authorize]
        public async Task<IActionResult> DeleteBug([FromBody] BugDTO bugDTO)
        {
            try
            {
                //Bounces user if user is not logged in
                if (!User.Identity.IsAuthenticated) return Unauthorized();

                //Looks for project by ID
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Bugs)
                    .FirstOrDefaultAsync(p => p.ID == bugDTO.ProjectID);
                if (project == null) return NotFound();

                //Gets bug in project
                var bug = project.Bugs.Find(b => b.ID == bugDTO.BugID);
                if (bug == null) return NotFound();

                //Gets user account
                var account = await _userManager.GetUserAsync(User);

                //Bounces user if they did not create the bug or are not the project owner
                if (project.Owner != account && bug.Creator != account) return Forbid();

                //Removes bug and saves changes
                project.Bugs.Remove(bug);
                project.DateModified = DateTime.UtcNow;
                await _databaseContext.SaveChangesAsync();
                return NoContent();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
