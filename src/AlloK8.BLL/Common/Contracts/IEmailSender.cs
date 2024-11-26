using System.Threading.Tasks;
using AlloK8.BLL.Common.Models;
using Essentials.Results;

namespace AlloK8.BLL.Common.Contracts;

internal interface IEmailSender
{
    Task<StandardResult> SendEmailAsync(EmailModel model);
}
