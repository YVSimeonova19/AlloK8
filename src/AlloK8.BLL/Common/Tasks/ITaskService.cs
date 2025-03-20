using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace AlloK8.BLL.Common.Tasks;

public interface ITaskService
{
    Task<DAL.Models.Task> CreateTaskAsync(TaskIM taskIM);

    Task<DAL.Models.Task> GetTaskByIdAsync(int id);
    Task<List<DAL.Models.Task>> GetAllTasksByProjectIdAsync(int projectId);
    Task<List<AlloK8.DAL.Models.Task>> GetAllTasksByAssigneeIdAsync(int projectId, int assigneeId);

    Task<DAL.Models.Task> UpdateTaskAsync(TaskUM taskUM, int id);

    Task<DAL.Models.Task> MoveTaskAsync(TaskUM taskUM, int id);

    Task AssignTaskAsync(int taskId, int userId);
    Task RemoveUserFromTaskAsync(int taskId, int userId);

    Task DeleteTaskByIdAsync(int id);

    Task<List<DAL.Models.Task>> PrioritizeTasksAsync(List<DAL.Models.Task> tasks);
}