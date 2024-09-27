using System.ComponentModel.DataAnnotations;

public class MakeBlogViewModel
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    public IFormFile? ProfilePicture { get; set; } 
    
    public List<IFormFile>? Images { get; set; } 
}
