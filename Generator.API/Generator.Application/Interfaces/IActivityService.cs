using Generator.Domain;

namespace Generator.Application.Interfaces;

public interface IActivityService
{
    void AddActivity(Activities activity);
    Activities GetActivityById(int id);
    void DeleteActivity(int id);
}
