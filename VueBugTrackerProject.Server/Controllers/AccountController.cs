using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace VueBugTrackerProject.Server.Controllers
{
    [ApiController]
    [Route("/accounts")]
    public class AccountController : ControllerBase
    {
        private DatabaseContext _context;
        private AuthService _authService;

        public AccountController(DatabaseContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        /// <summary>
        /// Function to check if a user in the app has a given username. 
        /// </summary>
        /// <param name="username">The username to be checked.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("verifyusername/{username}")]
        public async Task<IActionResult> VerifyUsername(string username)
        {
            try
            {
                var result = await (_context.Accounts.AnyAsync(a => a.Username.ToLower() == username.ToLower()));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
