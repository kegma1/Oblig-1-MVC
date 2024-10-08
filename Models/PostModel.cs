using System.ComponentModel.DataAnnotations;

public class Post
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public User? Author { get; set; }
    public Blog? blog { get; set; }
    public string? Image { get; set; }  
    public List<Comment> Comments { get; set; } = new List<Comment>();
}
