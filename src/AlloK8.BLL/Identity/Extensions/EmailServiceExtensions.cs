using System.Threading.Tasks;
using AlloK8.BLL.Common.EmailSending;
using AlloK8.Common;
using Essentials.Results;

namespace AlloK8.BLL.Identity.Extensions;

public static class EmailServiceExtensions
{
    public static async Task<StandardResult> SendResetPasswordEmailAsync(
        this IEmailService emailService,
        string email,
        string token)
    {
        return await emailService.SendEmailAsync(
            new EmailModel
            {
                Email = email,
                Subject = AlloK8.Common.T.ResetPasswordTitle,
                Message = string.Format(AlloK8.Common.Emails.ResetPassword, token),
            });
    }
}