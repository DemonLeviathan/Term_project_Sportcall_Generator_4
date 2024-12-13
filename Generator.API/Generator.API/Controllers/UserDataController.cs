using Generator.API.DTO;
using Generator.Domain;
using Generator.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // Получить данные пользователя по id
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
        [HttpPost]
        public async Task<IActionResult> AddUserData([FromBody] UserDataDto userDataDto)
        {
            if (userDataDto == null)
                return BadRequest("User data cannot be null.");

            // Поиск activity_id по имени
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.activity_name == userDataDto.activity_name);

            if (activity == null)
                return BadRequest($"Activity with name '{userDataDto.activity_name}' not found.");

            // Преобразование DTO в сущность
            var userData = new UserData
            {
                user_id = userDataDto.user_id,
                activity_id = activity.activity_id, // Используем найденный ID
                weight = userDataDto.weight,
                height = userDataDto.height
            };

            try
            {
                // Сохранение в базу данных
                _context.UserData.Add(userData);
                await _context.SaveChangesAsync();

                // Возврат результата
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
