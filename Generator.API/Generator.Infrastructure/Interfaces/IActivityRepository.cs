using Generator.Domain;

namespace Generator.Infrastructure.Interfaces;

public interface IActivityRepository
{
    void Add(Activities activity);
    Activities GetById(int id);
    void Remove(Activities activity);
}
