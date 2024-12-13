using Generator.API.DTO;
using Generator.Application.Interfaces;
using Generator.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Generator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : Controller
    {
        private readonly IActivityService _activityService;
        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }


        // add activity admin

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


    }
}
