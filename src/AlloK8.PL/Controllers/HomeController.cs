using System.Security.Claims;
using System.Threading.Tasks;
using AlloK8.BLL.Common.EmailSending;
using AlloK8.BLL.Identity.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AlloK8.PL.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService emailService;
    private readonly ICurrentUser currentUser;
    private readonly ILogger<HomeController> logger;

    public HomeController(
        IEmailService emailService,
        ICurrentUser currentUser,
        ILogger<HomeController> logger)
    {
        this.emailService = emailService;
        this.currentUser = currentUser;
        this.logger = logger;
    }

    [HttpGet("/")]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public IActionResult Index()
    {
        return this.View();
    }

    [HttpGet("/emailTest")]
    public async Task<IActionResult> SendConfirmationEmail()
    {
        var emailModel = new EmailModel
        {
            Email = "hwplatformaibest@gmail.com",
            Subject = "Confirmation Email",
            Message = "<h1>Thank you for registering!</h1><p>Your registration is complete.</p>",
        };

        await this.emailService.SendEmailAsync(emailModel);

        return this.Ok("Email Sent Successfully");
    }
}