using System.Collections.Generic;
using AlloK8.DAL.Models;

namespace AlloK8.BLL.Common.Tasks;

public interface ITaskService
{
    Task CreateTask(TaskIM taskIM);

    Task GetTaskById(int id);
    List<Task> GetAllTasks();

    Task UpdateTask(TaskUM taskUM, int id);

    // Think about those two ->
    Task MoveTask(TaskUM taskUM, int id);
    // Task AssignTask(TaskUM taskUM, int id);

    void DeleteTaskById(int id);
}