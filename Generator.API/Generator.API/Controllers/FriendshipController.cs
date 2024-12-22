using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Generator.Infrastructure; 
using Generator.Domain;
using Generator.API.DTO;

namespace Generator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FriendshipController : Controller
{
    private readonly ApplicationDbContext _context;

    public FriendshipController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddFriend([FromBody] FriendshipDto dto)
    {
        if (dto == null)
            return BadRequest("Friendship data is missing.");

        var exists = await _context.Friendships.AnyAsync(f =>
            (f.user1_id == dto.user1_id && f.user2_id == dto.user2_id) ||
            (f.user1_id == dto.user2_id && f.user2_id == dto.user1_id));

        if (exists)
            return BadRequest("Friendship already exists.");

        var friendship = new Friendship
        {
            user1_id = dto.user1_id,
            user2_id = dto.user2_id,
            friendship_date = DateTime.UtcNow.ToString("yyyy-MM-dd")
        };

        _context.Friendships.Add(friendship);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFriendshipById), new { id = friendship.friend_id }, friendship);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> RemoveFriend(int id)
    {
        var friendship = await _context.Friendships.FindAsync(id);
        if (friendship == null)
            return NotFound($"Friendship with id {id} not found.");

        _context.Friendships.Remove(friendship);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFriendshipById(int id)
    {
        var friendship = await _context.Friendships.FindAsync(id);
        if (friendship == null)
            return NotFound($"Friendship with id {id} not found.");

        return Ok(friendship);
    }
}
