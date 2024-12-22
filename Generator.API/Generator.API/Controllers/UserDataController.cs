using Generator.API.DTO;
using Generator.Domain;
using Generator.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Generator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDataController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserData(int id)
        {
            var userData = await _context.UserData
                .Include(ud => ud.Activity)  
                .FirstOrDefaultAsync(ud => ud.data_id == id);

            if (userData == null)
                return NotFound($"User data with ID '{id}' not found.");

            return Ok(userData);
        }

        // add user data
        [Authorize(Policy = "UserPolicy")]
        [HttpPost]
        public async Task<IActionResult> AddUserData([FromBody] UserDataDto userDataDto)
        {
            if (userDataDto == null)
                return BadRequest("User data cannot be null.");

            if (User == null || !User.Identity.IsAuthenticated)
            {
                return Unauthorized("User бляяяяяяяяяяя is not authorized.");
            }

            var token = Request.Headers["Authorization"].ToString();
            Console.WriteLine(token); 
            
                Console.WriteLine(User.Identity.Name);

            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }


            // TODO: FIND FIELD WITH KEY "SUB" AND TAKE USERNAME
            var userNameClaim = User.Claims.FirstOrDefault(); 

            if (userNameClaim == null)
                { return Unauthorized("User is not authorized."); }

            string userName = userNameClaim.Value;
            Console.WriteLine($"UserName from JWT: {userName}");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.username == userName); 

            if (user == null)
                return BadRequest($"User with name '{userName}' not found.");

            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.activity_name == userDataDto.activity_name);

            if (activity == null)
                return BadRequest($"Activity with name '{userDataDto.activity_name}' not found.");

            var userData = new UserData
            {
                user_id = user.user_id, 
                activity_id = activity.activity_id,
                weight = userDataDto.weight,
                height = userDataDto.height
            };

            try
            {
                _context.UserData.Add(userData);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUserData), new { id = userData.data_id }, userData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        // update user data
        // activity statistic
        // compare statistic
    }
}
