using System.ComponentModel.DataAnnotations;

public class RegisterViewModel {
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password  { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Password must match")]
    public  string ConfirmPassword { get; set; }

    [Required]
    public string Username { get; set; }
}