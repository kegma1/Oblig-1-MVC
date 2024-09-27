public class Blog
{
    public int Id { get; set; } 
    public string Title { get; set; } = "";
    public string Description { get; set; } = ""; 
    public List<Post>? Posts { get; set; } 
    public User? Author { get; set; } 
    public string? ProfilePicture { get; set; }  
}
