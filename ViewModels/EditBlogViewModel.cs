public class EditBlogViewModel {
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? BannerUrl  { get; set; }
    public IFormFile? NewBanner { get; set; } 

    public int blogId { get; set; }
}