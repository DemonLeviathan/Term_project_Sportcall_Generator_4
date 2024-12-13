using Generator.Application.Interfaces;
using Generator.Infrastructure.Interfaces;

namespace Generator.Application.Services;

public class FriendshipService : IFriendshipService
{
    private readonly IUnitOfWork unitOfWork;

    public FriendshipService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }
}
