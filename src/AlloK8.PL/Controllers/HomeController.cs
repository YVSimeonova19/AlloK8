using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AlloK8.BLL.Common.EmailSending;

namespace AlloK8.PL.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService emailService;

    public HomeController(IEmailService emailService)
    {
        this.emailService = emailService;
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