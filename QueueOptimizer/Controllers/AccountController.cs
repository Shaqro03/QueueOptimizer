using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.ConstrainedExecution;
using QueueOptimizer.Models;
using QueueOptimizer.ViewModels;

namespace QueueOptimizer.Controllers;

public class AccountController : Controller
{  
    private readonly UserManager<Users> userManager;
    private readonly SignInManager<Users> signInManager;
    private readonly IWebHostEnvironment webHostEnvironment; // Add environment for file handling


    public AccountController(UserManager<Users> userManager, SignInManager<Users> signInManager, IWebHostEnvironment webHostEnvironment)
    {
   
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Table");
            }
            else
            {
                ModelState.AddModelError("", "Email or password is incorrect.");
                return View(model);
            }
        }
        return View(model);
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            Users users = new Users
            {
                FullName = model.Name,
                Email = model.Email,
                UserName = model.Email,
                ProfilePicture = "/images/default-profile.png"
            };

            var result = await userManager.CreateAsync(users, model.Password);

            if (result.Succeeded)
            {
                  return RedirectToAction("Login", "Account");
                
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
        return View(model);
    }

    public IActionResult VerifyEmail()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Something is wrong!");
                return View(model);
            }
            else
            {
                return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
            }
        }
        return View(model);
    }

    public IActionResult ChangePassword(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("VerifyEmail", "Account");
        }
        return View(new ChangePasswordViewModel { Email = username });
    }
    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                var result = await userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    result = await userManager.AddPasswordAsync(user, model.NewPassword);
                    return RedirectToAction("Login", "Account");
                }
                else
                {

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Email not found!");
                return View(model);
            }
        }
        else
        {
            ModelState.AddModelError("", "Something went wrong. try again.");
            return View(model);
        }
    }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> Profile()
    {
        

        var user = await userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new ProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Password = "********",
            ProfilePicture = user.ProfilePicture ?? "/images/default-profile.png"

        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Login", "Account");

        // Update Full Name
        if (!string.IsNullOrWhiteSpace(model.FullName))
            user.FullName = model.FullName;

        // Update Email (if it's different)
        if (!string.IsNullOrWhiteSpace(model.Email) && model.Email != user.Email)
        {
            var token = await userManager.GenerateChangeEmailTokenAsync(user, model.Email);
            var emailResult = await userManager.ChangeEmailAsync(user, model.Email, token);

            if (!emailResult.Succeeded)
            {
                foreach (var error in emailResult.Errors)
                    ModelState.AddModelError("", error.Description);

                // Return the same view with validation messages
                return View("Profile", model);
            }
        }

        // Update Profile Picture
        if (model.ProfileImage != null)
        {
            // Optional: Remove previous image from server
            if (!string.IsNullOrEmpty(user.ProfilePicture) && !user.ProfilePicture.Contains("default-profile.png"))
            {
                string oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, user.ProfilePicture.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadFolder);
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ProfileImage.FileName);
            string filePath = Path.Combine(uploadFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.ProfileImage.CopyToAsync(fileStream);
            }

            user.ProfilePicture = "/uploads/" + fileName;
        }

        await userManager.UpdateAsync(user);

        return RedirectToAction("Profile");
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RemoveProfilePicture()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Remove the file from the server (optional)
        if (!string.IsNullOrEmpty(user.ProfilePicture))
        {
            string filePath = Path.Combine(webHostEnvironment.WebRootPath, user.ProfilePicture.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        // Reset profile picture to default
        user.ProfilePicture = "/images/default-profile.png"; 
        await userManager.UpdateAsync(user);

        return RedirectToAction("Profile");
    }



   


}

