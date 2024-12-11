using System.Threading.Tasks;
using Essentials.Results;

namespace AlloK8.BLL.Common.EmailSending;

public interface IEmailService
{
    Task<StandardResult> SendEmailAsync(EmailModel emailModel);
}
