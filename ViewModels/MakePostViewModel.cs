using System.ComponentModel.DataAnnotations;

public class MakePostViewModel
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public IFormFile? Image { get; set; } 
    public int blogId { get; set; }
}
