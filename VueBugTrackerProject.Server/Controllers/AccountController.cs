using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sodium;
using System.Diagnostics;
using VueBugTrackerProject.Classes;

namespace VueBugTrackerProject.Server.Controllers
{
    [ApiController]
    [Route("/accounts")]
    public class AccountController : ControllerBase
    {
        private DatabaseContext _context;
        private UserManager<Account> _userManager;
        private SignInManager<Account> _signInManager;

        public AccountController(DatabaseContext context, UserManager<Account> userManager, SignInManager<Account> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Function to check if a user in the app has a given username. 
        /// </summary>
        /// <param name="username">The username to be checked.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("verifyusername/{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyUsername(string username)
        {
            try
            {
                var result = await (_context.Accounts.AnyAsync(a => a.UserName.ToLower() == username.ToLower()));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Removes an account by an ID provided.
        /// </summary>
        /// <param name="accountId">The ID of the user to be deleted.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteaccount")]
        [Authorize]
        public async Task<IActionResult> DeleteUser([FromBody] string accountId)
         {
            //TODO: possibly allow admins and super user to delete accounts
            try
            {
                //Cancels the request if the ID supplied does not match the logged in user
                var currentUser = _userManager.GetUserId(User);
                if (currentUser != accountId) return Unauthorized("Invalid delete request.");

                var accountToRemove = await _context.Accounts.FindAsync(accountId);
                if (accountToRemove != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(accountToRemove);

                    await _signInManager.SignOutAsync();

                    await _userManager.RemoveFromRolesAsync(accountToRemove, userRoles);
                    await _userManager.DeleteAsync(accountToRemove);
                    return NoContent();
                }
                return NotFound("No account exists with the ID provided.");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
