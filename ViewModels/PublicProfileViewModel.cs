public class PublicProfileViewModel {
    public string? Username { get; set; }
    public string? bio { get; set; }
    public string? ProfilePictureUrl  { get; set; }

    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }

    public IEnumerable<Blog>? Blogs { get; set; }
}