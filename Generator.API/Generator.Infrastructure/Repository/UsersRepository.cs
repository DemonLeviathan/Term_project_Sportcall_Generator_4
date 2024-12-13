using Generator.Infrastructure.Interfaces;

namespace Generator.Infrastructure.Repository;

public class UsersRepository : IUsersRepository
{
    private readonly ApplicationDbContext _context;

    public UsersRepository(ApplicationDbContext context)
    {
        _context = context;
    }
}
