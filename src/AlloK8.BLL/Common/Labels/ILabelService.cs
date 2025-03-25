using System.Collections.Generic;
using System.Threading.Tasks;
using AlloK8.Common.Models.Label;
using AlloK8.DAL.Models;
using Task = System.Threading.Tasks.Task;

namespace AlloK8.BLL.Common.Labels;

public interface ILabelService
{
    Task<Label> CreateLabelAsync(LabelIM labelIM);

    Task<Label> GetLabelByIdAsync(int id);
    Task<List<Label>> GetLabelsByTaskIdAsync(int taskId);
    Task<List<Label>> GetLabelsByProjectIdAsync(int projectId);

    Task<Label> EditLabelAsync(LabelUM labelUM, int labelId);

    Task DeleteLabelAsync(int id);
}