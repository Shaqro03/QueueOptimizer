using System.ComponentModel.DataAnnotations;

namespace QueueOptimizer.ViewModels;

public class ProfileViewModel
{
   

    [Display(Name = "Full Name")]
    public string FullName { get; set; }

    [Display(Name = "Email")]
    public string Email { get; set; }

    [Display(Name = "Password")]
    public string Password { get; set; }

    public string ProfilePicture { get; set; } 

    [Display(Name = "Upload New Profile Picture")]
    public IFormFile? ProfileImage { get; set; }
}
