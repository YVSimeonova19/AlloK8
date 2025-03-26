using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AlloK8.Common.Models.Project;

namespace AlloK8.BLL.Common.Invoices;

public interface IReportService
{
    Task<DataTable> GetProjectProgressAsync(int projectId);
}