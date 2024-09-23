public class Blog {
    public int Id { get; set; } 
    public string Title { get; set; } = "";
    public string desctiption { get; set; } = ""; 
    public List<Post>? posts { get; set; } 
    public User? Author { get; set; } 
}

