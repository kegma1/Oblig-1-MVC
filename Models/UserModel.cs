using Microsoft.AspNetCore.Identity;

public class User : IdentityUser {        
    public string Bio { get; set; } = "";                 
    public string ProfilePicture { get; set; } = "Default_pfp.jpg";      
    public bool IsAdmin { get; set; }                
    public bool IsDeleted { get; set; }              

    
    public List<User> Followers { get; set; } = new List<User>();        
    public List<User> Following { get; set; } = new List<User>();        

    public List<Blog>? Blogs { get; set; }   

}