using System.Collections.Generic;
using System.Threading.Tasks;
using AlloK8.Common.Models.Report;

namespace AlloK8.BLL.Common.Reports;

public interface IReportService
{
    Task<List<ReportVM>> GetProjectProgressAsync(int projectId);

    Task<byte[]> GenerateProjectReportPdfAsync(int projectId, string projectName);
}