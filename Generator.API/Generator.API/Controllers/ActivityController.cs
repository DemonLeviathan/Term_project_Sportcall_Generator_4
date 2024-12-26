using Generator.API.DTO;
using Generator.Application.Interfaces;
using Generator.Domain;
using Generator.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Generator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : Controller
    {
        private readonly IActivityService _activityService;
        private readonly IUnitOfWork _unitOfWork;
        public ActivityController(IActivityService activityService, IUnitOfWork unitOfWork)
        {
            _activityService = activityService;
            _unitOfWork = unitOfWork;
        }


        // add activity admin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddActivity([FromBody] ActivityDto activityDto)
        {
            if (activityDto == null)
            {
                return BadRequest("Activity data is required.");
            }

            var activity = new Activities
            {
                activity_name = activityDto.activity_name,
                activity_type = activityDto.activity_type
            };

            _activityService.AddActivity(activity);

            return CreatedAtAction(nameof(GetActivityById), new { id = activity.activity_id }, activity);
        }

        [HttpGet("{id}")]
        public IActionResult GetActivityById(int id)
        {
            var activity = _activityService.GetActivityById(id);
            if (activity == null)
            {
                return NotFound();
            }

            return Ok(activity);
        }


        //delete activity admin
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteActivity(int id)
        {
            var activity = _activityService.GetActivityById(id);
            if (activity == null)
            {
                return NotFound($"Activity with ID {id} not found.");
            }

            _activityService.DeleteActivity(id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public IActionResult GetAllUsers([FromQuery] string? searchTerm)
        {
            var users = _unitOfWork.Users.GetAll();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u => u.username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("statistics")]
        public IActionResult GetOverallStatistics()
        {
            var statistics = _unitOfWork.Users.GetAll()
                .Select(u => new
                {
                    u.username,
                    ActivitiesCount = u.UserData.Count,
                    FriendsCount = u.Friendships1.Count + u.Friendships2.Count
                });

            return Ok(statistics);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("statistics/{id}")]
        public IActionResult GetUserStatistics(int id)
        {
            var user = _unitOfWork.Users.GetById(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            var statistics = new
            {
                user.username,
                ActivitiesCount = user.UserData.Count,
                FriendsCount = user.Friendships1.Count + user.Friendships2.Count
            };

            return Ok(statistics);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("activities")]
        public IActionResult GetAllActivities([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var activities = _activityService.GetAllActivities();
            var paginatedActivities = activities
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalPages = (int)Math.Ceiling((double)activities.Count() / pageSize);

            return Ok(new
            {
                activities = paginatedActivities,
                totalPages = totalPages
            });
        }


        [HttpGet("types")]
        public IActionResult GetAllActivityTypes()
        {
            var activityTypes = _activityService.GetAllActivities()
                .Select(a => a.activity_type)
                .Distinct()
                .ToList();

            return Ok(activityTypes);
        }

        [HttpGet("by-type")]
        public IActionResult GetActivitiesByType([FromQuery] string activityType)
        {
            if (string.IsNullOrWhiteSpace(activityType))
            {
                return BadRequest("Activity type is required.");
            }

            var activities = _activityService.GetAllActivities()
                .Where(a => a.activity_type.Equals(activityType, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!activities.Any())
            {
                return NotFound($"No activities found for type '{activityType}'.");
            }

            return Ok(activities);
        }
    }
}
