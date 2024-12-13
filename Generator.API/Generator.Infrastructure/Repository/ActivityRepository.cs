using Generator.Domain;
using Generator.Infrastructure.Interfaces;

namespace Generator.Infrastructure.Repository;

public class ActivityRepository : IActivityRepository
{
    private readonly ApplicationDbContext _context;

    public ActivityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(Activities activity)
    {
        if (activity == null)
        {
            throw new ArgumentNullException(nameof(activity), "Activity cannot be null");
        }

        _context.Activities.Add(activity); 
    }

    public Activities GetById(int id)
    {
        return _context.Activities.FirstOrDefault(a => a.activity_id == id); 
    }

    public void Remove(Activities activity)
    {
        _context.Activities.Remove(activity);
    }
}
