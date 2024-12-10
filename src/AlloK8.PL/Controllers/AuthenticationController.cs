using System.Threading.Tasks;
using AlloK8.DAL;
using AlloK8.PL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AlloK8.PL.Controllers;

public class AuthenticationController : Controller
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly SignInManager<ApplicationUser> signInManager;

    public AuthenticationController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    [HttpGet("/login")]
    public IActionResult Login()
    {
        var model = new LoginVM();
        return this.View(model);
    }

    [HttpPost("/login")]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginVM model)
    {
        if (this.ModelState.IsValid)
        {
            // do the login
        }

        return this.View(model);
    }

    [HttpGet("/register")]
    public IActionResult Register()
    {
        var model = new RegisterVM();
        return this.View(model);
    }

    [HttpPost("/register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM model)
    {
        if (this.ModelState.IsValid)
        {
            var result = await this.userManager.CreateAsync(
                new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                },
                model.Password!);
            if (result.Succeeded)
            {
                this.TempData["MessageText"] = "User created a new account with password.";
                this.TempData["MessageVariant"] = "success";
                return this.RedirectToAction("Login");
            }
        }

        return this.View(model);
    }

    [HttpGet("/forgot-password")]
    public IActionResult ForgotPassword()
    {
        var model = new ForgotPasswordVM();
        return this.View(model);
    }

    [HttpPost("/forgot-password")]
    [ValidateAntiForgeryToken]
    public IActionResult ForgotPassword(ForgotPasswordVM model)
    {
        if (this.ModelState.IsValid)
        {
            // do the fp
        }

        return this.View(model);
    }
}