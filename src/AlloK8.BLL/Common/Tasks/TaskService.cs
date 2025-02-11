using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.DAL;
using Task = System.Threading.Tasks.Task;

namespace AlloK8.BLL.Common.Tasks;

internal class TaskService : ITaskService
{
    private readonly EntityContext context;

    public TaskService(EntityContext context)
    {
        this.context = context;
    }

    public async Task<DAL.Models.Task> CreateTask(TaskIM taskIM)
    {
        var task = new DAL.Models.Task
        {
            Title = taskIM.Title,
            Description = taskIM.Description,
            DueDate = taskIM.DueDate ?? DateTime.Now,
            Position = 0,
            ColumnId = 1,
            StartDate = DateTime.Now,
        };

        this.context.Tasks.Add(task);
        this.context.SaveChanges();

        return task;
    }

    public async Task<DAL.Models.Task> GetTaskById(int id)
    {
        var task = this.context.Tasks.Find(id);
        if (task == null)
        {
            throw new KeyNotFoundException("Task not found");
        }

        return task;
    }

    public async Task<List<DAL.Models.Task>> GetAllTasks()
    {
        return this.context.Tasks.ToList();
    }

    public async Task<DAL.Models.Task> UpdateTask(TaskUM taskUM, int id)
    {
        var task = await this.GetTaskById(id);

        task.Title = taskUM.Title ?? task.Title;
        task.Description = taskUM.Description ?? task.Description;
        task.Position = taskUM.Position ?? task.Position;
        task.DueDate = taskUM.DueDate ?? task.DueDate;

        this.context.Tasks.Update(task);
        this.context.SaveChanges();

        return task;
    }

    public async Task<DAL.Models.Task> MoveTask(TaskUM taskUM, int id)
    {
        var task = await this.GetTaskById(id);

        task.Position = taskUM.Position ?? task.Position;
        task.ColumnId = taskUM.ColumnId ?? task.ColumnId;

        this.context.Tasks.Update(task);
        this.context.SaveChanges();

        return task;
    }

/*
    public async Task<DAL.Models.Task> AssignTask(TaskUM taskUM, int id)
    {
        var task = await this.GetTaskById(id);

        // Assuming taskUM.Assignees is a list of user IDs
        foreach (var userId in taskUM.Assignees)
        {
            var user = this.context.UserProfiles.Find(userId);
            if (user != null)
            {
                task.Assignees.Add(user);
            }
        }

        this.context.Tasks.Update(task);
        this.context.SaveChanges();

        return task;
    }
*/
    public async Task DeleteTaskById(int id)
    {
        var task = await this.GetTaskById(id);

        this.context.Tasks.Remove(task);
        this.context.SaveChanges();
    }
}