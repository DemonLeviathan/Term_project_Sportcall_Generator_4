namespace Generator.Domain;

public class Users
{
    public int user_id { get; set; }
    public string username { get; set; }
    public string password { get; set; }
    public string birthday { get; set; }

    public ICollection<UserData> UserData { get; set; }
    public ICollection<Friendship> Friendships1 { get; set; } 
    public ICollection<Friendship> Friendships2 { get; set; } 

    public ICollection<Calls> Calls { get; set; }
}
