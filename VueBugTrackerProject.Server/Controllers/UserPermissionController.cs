using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VueBugTrackerProject.Classes;

namespace VueBugTrackerProject.Server.Controllers
{
    /// <summary>
    /// Controller for adding, viewing and editing user permmissions
    /// for a project.
    /// </summary>
    [ApiController]
    [Route("userpermissions")]
    public class UserPermissionController : ControllerBase
    {
       private readonly UserManager<Account> _userManager;
       private readonly DatabaseContext _databaseContext;

        public UserPermissionController(UserManager<Account> userManager, DatabaseContext databaseContext)
        {
            _userManager = userManager;
            _databaseContext = databaseContext;
        }

        [HttpPost]
        [Route("add")]
        [Authorize]
        public async Task<IActionResult> AddUserPermission([FromBody] UserPermissionDTO permissionDTO)
        {
            try
            {
                //Looks for project by ID
                var project = await _databaseContext.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.UserPermissions)
                    .FirstOrDefaultAsync(p => p.ID == permissionDTO.ProjectID);

                //Returns error if project does not exist
                if (project == null) return NotFound("Project does not exist");

                //Checks if user owns the project
                var account = await _userManager.GetUserAsync(User);
                if (project.Owner != account) return Forbid();

                //Looks for user
                var userToAdd = await _databaseContext.Accounts.FirstOrDefaultAsync(u => u.Id == permissionDTO.AccountID);
                if (userToAdd == null) return NotFound();

                //Checks if user already has a permission for the project or owns the project
                if(project.UserPermissions.Any(up => up.Account ==  account) || userToAdd == account) return BadRequest();

                //Adds and saves permission
                project.UserPermissions.Add(new UserPermission
                {
                    Account = userToAdd,
                    Permission = permissionDTO.Permission
                });
                await _databaseContext.SaveChangesAsync();

                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
