
namespace Generator.Domain
{
    public class Calls
    {
        public int call_id {  get; set; }
        public string call_name { get; set; }
        public int friend_id { get; set; }
        public string call_date {  get; set; }
        public string status { get; set; }

        public Friendship Friendship { get; set; }
    }
}
