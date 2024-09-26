public class EditProfileViewModel {
    public string? Username { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl  { get; set; }
    public IFormFile? NewProfilePicture { get; set; } 
}