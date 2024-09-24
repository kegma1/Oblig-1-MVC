public class Blog
{
    public int Id { get; set; } 
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";  // Fix typo here
    public List<Post>? Posts { get; set; } 
    public User? Author { get; set; } 
    public string? ProfilePicture { get; set; }  // Ensure ProfilePicture is present
}
