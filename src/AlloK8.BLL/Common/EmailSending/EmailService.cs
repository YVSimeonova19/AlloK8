using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace AlloK8.BLL.Common.EmailSending;

internal class EmailService : IEmailService
{
    private readonly EmailSendGridOptions options;

    public EmailService(IOptions<EmailSendGridOptions> options)
    {
        this.options = options.Value;
    }

    public async Task SendEmailAsync(EmailModel emailModel)
    {
        var client = new SendGridClient(options.ApiKey);

        var sender = new EmailAddress(options.Email, options.Name);

        var recipient = new EmailAddress(emailModel.Email);

        var message = MailHelper.CreateSingleEmail(sender, recipient, emailModel.Subject, emailModel.Message, emailModel.Message);

        var response = await client.SendEmailAsync(message);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Email sent");
        }
        else
        {
            Console.WriteLine("Error sending email: " + response.StatusCode.ToString());
        }
    }
}