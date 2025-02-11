using System.Security.Claims;
using System.Threading.Tasks;
using AlloK8.BLL.Common.EmailSending;
using AlloK8.BLL.Identity.Constants;
using AlloK8.BLL.Identity.Contracts;
using AlloK8.DAL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AlloK8.PL.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class HomeController : Controller
{
    private readonly IEmailService emailService;
    private readonly ICurrentUser currentUser;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ILogger<HomeController> logger;

    public HomeController(
        IEmailService emailService,
        ICurrentUser currentUser,
        UserManager<ApplicationUser> userManager,
        ILogger<HomeController> logger)
    {
        this.emailService = emailService;
        this.currentUser = currentUser;
        this.userManager = userManager;
        this.logger = logger;
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        return this.View();
    }

    [HttpGet("/emailTest")]
    public async Task<IActionResult> SendConfirmationEmail()
    {
        var emailModel = new EmailModel
        {
            Email = "vonjoanna2005@gmail.com",
            Subject = "Confirmation Email",
            Message = "<h1>Thank you for registering!</h1><p>Your registration is complete.</p>",
        };

        await this.emailService.SendEmailAsync(emailModel);

        return this.Ok("Email Sent Successfully");
    }
}