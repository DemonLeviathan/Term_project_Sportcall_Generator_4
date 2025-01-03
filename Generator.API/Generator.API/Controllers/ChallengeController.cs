// Controllers/ChallengeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Generator.Infrastructure;
using Generator.Domain;
using Generator.API.DTO;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Generator.API.Controllers
{
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

            bool areFriends = await _context.Friendships.AnyAsync(f =>
                ((f.user1_id == dto.SenderId && f.user2_id == dto.ReceiverId) ||
                 (f.user1_id == dto.ReceiverId && f.user2_id == dto.SenderId)) &&
                !f.IsPending);


            if (!areFriends)
                return BadRequest("Пользователи не являются друзьями.");

            var call = await _context.Calls.FindAsync(dto.CallId);
            if (call == null)
                return NotFound("Вызов не найден.");

            bool challengeExists = await _context.Challenges.AnyAsync(c =>
                c.SenderId == dto.SenderId &&
                c.ReceiverId == dto.ReceiverId &&
                c.CallId == dto.CallId &&
                c.Status == "Pending");

            if (challengeExists)
                return BadRequest("Вызов уже отправлен и находится в ожидании.");

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
            if (dto == null)
                return BadRequest("Данные для ответа отсутствуют.");

            var challenge = await _context.Challenges
                .Include(c => c.Call)
                .FirstOrDefaultAsync(c => c.ChallengeId == dto.ChallengeId);

            if (challenge == null)
                return NotFound("Вызов не найден.");

            if (challenge.Status != "Pending")
                return BadRequest("На вызов уже был дан ответ.");

            if (dto.Accept)
            {
                challenge.Status = "Accepted";
                challenge.RespondedAt = DateTime.UtcNow;

                var receiverCall = new UserCall
                {
                    UserId = challenge.ReceiverId,
                    CallId = challenge.CallId,
                    Status = "Accepted",
                    AssignedAt = DateTime.UtcNow
                };

                _context.UserCalls.Add(receiverCall);
            }
            else
            {
                challenge.Status = "Rejected";
                challenge.RespondedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(dto.Accept ? "Вызов принят." : "Вызов отклонён.");
        }
    }
}