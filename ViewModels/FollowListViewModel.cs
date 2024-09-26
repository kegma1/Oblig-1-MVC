public class FollowListViewModel
{
    public string Username { get; set; }
    public string UserId { get; set; }
    public string ListType { get; set; } 
    public IEnumerable<User> Users { get; set; }
}
