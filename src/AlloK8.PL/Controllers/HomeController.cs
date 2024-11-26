using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AlloK8.PL.Models;
using Microsoft.Extensions.Logging;
using AlloK8.BLL.Common.Constants;
using AlloK8.BLL.Common.Contracts;
using AlloK8.BLL.Common.Models;
using System.Threading.Tasks;

namespace AlloK8.PL.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService _emailService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IEmailService emailService,
        ILogger<HomeController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index(string strategy = EmailSenderStrategies.NoOps)
    {
        var emailSent = await _emailService.SendEmailAsync(
            new EmailModel
            {
                Subject = "Welcome to EduSystem!",
                Email = "example_user@edusystem.dev",
                Message = $"You have received email with strategy {strategy}."
            },
            strategy);

        return Ok(emailSent);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}