public class PublicProfileViewModel
{
    public string UserId { get; set; } 
    public string Username { get; set; }
    public string Bio { get; set; }
    public string ProfilePictureUrl { get; set; }
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public List<Blog> Blogs { get; set; }
}
