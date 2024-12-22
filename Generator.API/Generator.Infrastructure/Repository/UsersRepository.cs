using Generator.Domain;
using Generator.Infrastructure.Interfaces;

namespace Generator.Infrastructure.Repository;

public class UsersRepository : IUsersRepository
{
    private readonly ApplicationDbContext _context;

    public UsersRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(Users user)
    {
        _context.Users.Add(user);
    }

    public Users GetByUsername(string username)
    {
        return _context.Users.FirstOrDefault(u => u.username == username);
    }

    public bool UserExists(string username)
    {
        return _context.Users.Any(u => u.username == username);
    }
}