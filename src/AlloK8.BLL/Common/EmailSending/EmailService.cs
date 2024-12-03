using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace AlloK8.BLL.Common.EmailSending
{
    internal class EmailService : IEmailService
    {
        private readonly EmailSendGridOptions emailOptions;

        public EmailService(IOptions<EmailSendGridOptions> options)
        {
            emailOptions = options.Value ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(emailOptions.ApiKey))
                throw new Exception("SendGrid API key is not configured.");
            if (string.IsNullOrEmpty(emailOptions.Email))
                throw new Exception("SendGrid Email is not configured.");
            if (string.IsNullOrEmpty(emailOptions.Name))
                throw new Exception("SendGrid Name is not configured.");
        }

        public async Task SendEmailAsync(EmailModel emailModel)
        {
            var client = new SendGridClient(emailOptions.ApiKey);

            var sender = new EmailAddress(emailOptions.Email, emailOptions.Name);

            var recipient = new EmailAddress(emailModel.Email);

            var message = MailHelper.CreateSingleEmail(sender, recipient, emailModel.Subject, emailModel.Message,
                emailModel.Message);

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
}