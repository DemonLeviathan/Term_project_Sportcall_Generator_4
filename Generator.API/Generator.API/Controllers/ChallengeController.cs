using Generator.API.DTO;
using Generator.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Generator.Domain;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "User,Admin")]
public class ChallengeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ChallengeController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Отправить вызов другу.
    /// </summary>
    [HttpPost("send")]
    public async Task<IActionResult> SendChallenge([FromBody] SendChallengeDto dto)
    {
        if (dto == null)
            return BadRequest("Данные вызова отсутствуют.");

        var challenge = new Challenge
        {
            SenderId = dto.SenderId,
            ReceiverId = dto.ReceiverId,
            CallId = dto.CallId,
            Status = "Pending",
            SentAt = DateTime.UtcNow
        };

        _context.Challenges.Add(challenge);
        await _context.SaveChangesAsync();

        return Ok("Вызов успешно отправлен.");
    }

    /// <summary>
    /// Получить все полученные вызовы для пользователя.
    /// </summary>
    [HttpGet("received")]
    public async Task<IActionResult> GetReceivedChallenges([FromQuery] int userId)
    {
        var challenges = await _context.Challenges
            .Where(c => c.ReceiverId == userId && c.Status == "Pending")
            .Include(c => c.Sender)
            .Include(c => c.Call)
            .Select(c => new
            {
                c.ChallengeId,
                SenderName = c.Sender.username,
                CallName = c.Call.call_name,
                CallDescription = c.Call.description,
                c.SentAt
            })
            .ToListAsync();

        if (!challenges.Any())
            return NotFound("Вызовов нет.");

        return Ok(challenges);
    }

    /// <summary>
    /// Ответить на вызов (принять или отклонить).
    /// </summary>
    [HttpPost("respond")]
    public async Task<IActionResult> RespondToChallenge([FromBody] RespondChallengeDto dto)
    {
        var challenge = await _context.Challenges
            .Include(c => c.Call) 
            .FirstOrDefaultAsync(c => c.ChallengeId == dto.ChallengeId);

        if (challenge == null)
            return NotFound("Вызов не найден.");

        if (challenge.Status != "Pending")
            return BadRequest("На вызов уже был дан ответ.");

        if (dto.Accept)
        {
            challenge.Status = "accepted";
            challenge.RespondedAt = DateTime.UtcNow;

            var duplicatedCall = new Calls
            {
                call_name = challenge.Call.call_name,
                description = challenge.Call.description,
                call_date = challenge.Call.call_date,
                status = "accepted", 
                user_id = challenge.ReceiverId 
            };

            _context.Calls.Add(duplicatedCall);
        }
        else
        {
            challenge.Status = "rejected";
            challenge.RespondedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return Ok(dto.Accept ? "Вызов принят." : "Вызов отклонён.");
    }


    /// <summary>
    /// Получить уведомления о вызовах.
    /// </summary>
    [HttpGet("notifications")]
    public async Task<IActionResult> GetChallengeNotifications([FromQuery] int userId)
    {
        var notifications = await _context.Challenges
            .Where(c => c.ReceiverId == userId && c.Status == "Pending")
            .Include(c => c.Sender)
            .Include(c => c.Call)
            .Select(c => new
            {
                c.ChallengeId,
                CallName = c.Call.call_name,
                Description = c.Call.description,
                SenderName = c.Sender.username,
                c.SenderId,
                c.ReceiverId,
                c.SentAt
            })
            .ToListAsync();

        if (!notifications.Any())
            return NotFound("Нет уведомлений о вызовах.");


        return Ok(notifications);
    }
}
