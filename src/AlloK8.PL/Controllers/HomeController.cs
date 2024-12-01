using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AlloK8.PL.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using AlloK8.BLL.Common.EmailSending;

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
    public async Task<IActionResult> Index()
    {
        await _emailService.SendEmailAsync(
            new EmailModel
            {
                Subject = "Welcome to AlloK8!",
                Email = "allok8.customerservice@gmail.com",
                Message = "You have received an email via SendGrid."
            });

        return Ok();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}