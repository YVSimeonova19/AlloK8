using System.Threading.Tasks;

namespace AlloK8.BLL.Common.EmailSending;

public interface IEmailService
{
    Task SendEmailAsync(EmailModel emailModel);
}
