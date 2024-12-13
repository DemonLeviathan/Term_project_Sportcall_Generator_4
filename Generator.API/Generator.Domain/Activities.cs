namespace Generator.Domain;

public class Activities
{
    public int activity_id {  get; set; }
    public string activity_name { get; set; }
    public string activity_type { get; set; }

    public ICollection<UserData> UserData { get; set; }
}
