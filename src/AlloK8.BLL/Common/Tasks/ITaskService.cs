using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace AlloK8.BLL.Common.Tasks;

public interface ITaskService
{
    Task<DAL.Models.Task> CreateTask(TaskIM taskIM);

    Task<DAL.Models.Task> GetTaskById(int id);
    Task<List<DAL.Models.Task>> GetAllTasks();

    Task<DAL.Models.Task> UpdateTask(TaskUM taskUM, int id);

    // Think about those two ->
    Task<DAL.Models.Task> MoveTask(TaskUM taskUM, int id);
    // Task AssignTask(TaskUM taskUM, int id);

    Task DeleteTaskById(int id);
}