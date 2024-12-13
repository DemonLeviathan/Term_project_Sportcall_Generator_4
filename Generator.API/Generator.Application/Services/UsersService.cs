using Generator.Application.Interfaces;
using Generator.Infrastructure.Interfaces;

namespace Generator.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUnitOfWork unitOfWork;

    public UsersService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }
}
