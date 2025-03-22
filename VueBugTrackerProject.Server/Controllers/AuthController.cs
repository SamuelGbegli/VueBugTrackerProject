using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using VueBugTrackerProject.Classes;

using Microsoft.IdentityModel.JsonWebTokens;

using Sodium;

namespace VueBugTrackerProject.Server.Controllers
{
    //TODO: add list of function returns
    /// <summary>
    /// Controller for processing auth requests.
    /// </summary>
    [ApiController]
    [Route("/auth")]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// The app's database context.
        /// </summary>
        private readonly DatabaseContext _context;

        private AuthService _authService;

        public AuthController(DatabaseContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        
        /// <summary>
        /// Function to log in the user to the application.
        /// </summary>
        /// <param name="userDTO">A DTO containing a username and password.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            try
            {
                //Searches for a user by the username input
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Username.ToLower() == userDTO.Username.ToLower());
                if (account == null)
                {
                    Trace.WriteLine("User does not exist.");
                    return Unauthorized("Incorrect username or password.");
                }

                //Checks if the user is suspended
                if (account.Suspended)
                {
                    Trace.WriteLine("Account has been suspended.");
                    return Unauthorized("Account has been suspended. Please contact an administrator if you need to regain access.");
                }

                //Checks if the inputted password matches the hash stored
                if (!PasswordHash.ArgonHashStringVerify(account.PasswordHash, userDTO.Password))
                {
                    Trace.WriteLine("Password does not match.");
                    return Unauthorized("Incorrect username or password.");
                }

                //Returns account ID, username and token if username and password match
                return Ok(new { id = account.ID, username = account.Username, token = _authService.GenerateToken(account) });
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Adds a new account to the database.
        /// </summary>
        /// <param name="userDTO">The user detatils that will be added to the database.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("createaccount")]
        public async Task<IActionResult> CreateAccount([FromBody] UserDTO userDTO)
        {
            try
            {
                var account = new Account
                {
                    Email = userDTO.EmailAddress,
                    Username = userDTO.Username,
                    PasswordHash = PasswordHash.ArgonHashString(userDTO.Password),
                    Role = AccountRole.Normal,
                    DateCreated = DateTime.Now
                };
                await _context.Accounts.AddAsync(account);
                await _context.SaveChangesAsync();
                Trace.WriteLine($"Successfully created user {userDTO.Username}");

                return Created();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("deleteaccount")]
        public async Task<IActionResult> DeleteUser([FromBody] string accountId)
        {
            try
            {
                var accountToRemove = await _context.Accounts.FindAsync(accountId);
                if (accountToRemove != null)
                {
                    _context.Accounts.Remove(accountToRemove);
                    await _context.SaveChangesAsync();
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

        /// <summary>
        /// Function to validate a user's token.
        /// </summary>
        /// <param name="token">The token to be validated.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("validatetoken")]
        public async Task<IActionResult> ValidateToken([FromBody] string token)
        {
            try
            {
                if (await _authService.ValidateToken(token)) return Ok();
                else return Unauthorized("Token is invalid or expired.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
