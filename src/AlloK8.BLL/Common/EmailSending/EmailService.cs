using System;
using System.Threading.Tasks;
using Essentials.Results;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AlloK8.BLL.Common.EmailSending
{
    internal class EmailService : IEmailService
    {
        private readonly EmailSendGridOptions emailOptions;

        public EmailService(IOptions<EmailSendGridOptions> options)
        {
            this.emailOptions = options.Value ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(this.emailOptions.ApiKey))
            {
                throw new Exception("SendGrid API key is not configured.");
            }

            if (string.IsNullOrEmpty(this.emailOptions.Email))
            {
                throw new Exception("SendGrid Email is not configured.");
            }

            if (string.IsNullOrEmpty(this.emailOptions.Name))
            {
                throw new Exception("SendGrid Name is not configured.");
            }
        }

        public async Task<StandardResult> SendEmailAsync(EmailModel emailModel)
        {
            var client = new SendGridClient(this.emailOptions.ApiKey);

            var sender = new EmailAddress(this.emailOptions.Email, this.emailOptions.Name);

            var recipient = new EmailAddress(emailModel.Email);

            var message = MailHelper.CreateSingleEmail(
                sender,
                recipient,
                emailModel.Subject,
                emailModel.Message,
                emailModel.Message);

            var response = await client.SendEmailAsync(message);

            return StandardResult.ResultFrom(response.IsSuccessStatusCode);
        }
    }
}