using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.DAL;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace AlloK8.BLL.Common.Tasks;

internal class TaskService : ITaskService
{
    private readonly EntityContext context;

    public TaskService(EntityContext context)
    {
        this.context = context;
    }

    public async Task<DAL.Models.Task> CreateTaskAsync(TaskIM taskIM)
    {
        // Get the maximum position in the target column
        int maxPosition = await this.context.Tasks
            .Where(t => t.ColumnId == taskIM.ColumnId)
            .Select(t => t.Position)
            .DefaultIfEmpty()
            .MaxAsync();

        var task = new DAL.Models.Task
        {
            Title = taskIM.Title,
            Description = taskIM.Description,
            DueDate = taskIM.DueDate ?? DateTime.Now,
            Position = Math.Max(maxPosition + 1, 1),
            ColumnId = taskIM.ColumnId ?? 1,
            ProjectId = taskIM.ProjectId,
            StartDate = DateTime.Now,
            CreatedByUserId = taskIM.CreatorId,
            CreatedOn = taskIM.CreatedOn,
            UpdatedByUserId = taskIM.CreatorId,
            UpdatedOn = taskIM.CreatedOn,
        };

        this.context.Tasks.Add(task);
        this.context.SaveChanges();

        return task;
    }

    public async Task<DAL.Models.Task> GetTaskByIdAsync(int id)
    {
        var task = this.context.Tasks.Find(id);
        if (task == null)
        {
            throw new KeyNotFoundException("Task not found");
        }

        return task;
    }

    public async Task<List<DAL.Models.Task>> GetAllTasksAsync()
    {
        return this.context.Tasks.ToList();
    }

    public async Task<List<DAL.Models.Task>> GetAllTasksByProjectIdAsync(int projectId)
    {
        return this.context.Tasks
            .Where(t => t.Project.Id == projectId)
            .ToList();
    }

    public async Task<DAL.Models.Task> UpdateTaskAsync(TaskUM taskUM, int id)
    {
        var task = await this.GetTaskByIdAsync(id);

        task.Title = taskUM.Title ?? task.Title;
        task.Description = taskUM.Description ?? task.Description;
        task.Position = taskUM.Position ?? task.Position;
        task.DueDate = taskUM.DueDate ?? task.DueDate;

        this.context.Tasks.Update(task);
        this.context.SaveChanges();

        return task;
    }

    public async Task<DAL.Models.Task> MoveTaskAsync(TaskUM taskUM, int id)
    {
        var task = await this.GetTaskByIdAsync(id);

        // Moving to a new column
        if (taskUM.ColumnId.HasValue && taskUM.ColumnId != task.ColumnId)
        {
            // Get the maximum position in the new column
            int maxPosition = await this.context.Tasks
                .Where(t => t.ColumnId == taskUM.ColumnId)
                .Select(t => t.Position)
                .DefaultIfEmpty()
                .MaxAsync();

            // Calculate the new position
            int newPosition = taskUM.Position ?? maxPosition + 1;

            // Ensure the position is at least 1
            newPosition = Math.Max(newPosition, 1);

            // Shift positions in the new column to make space
            var tasksInNewColumn = this.context.Tasks
                .Where(t => t.ColumnId == taskUM.ColumnId && t.Position >= newPosition)
                .ToList();

            foreach (var t in tasksInNewColumn)
            {
                t.Position++;
            }

            // Shift positions in the old column to fill the gap
            var tasksInOldColumn = this.context.Tasks
                .Where(t => t.ColumnId == task.ColumnId && t.Position > task.Position)
                .ToList();

            foreach (var t in tasksInOldColumn)
            {
                t.Position--;
            }

            task.ColumnId = taskUM.ColumnId.Value;
            task.Position = newPosition;
        }
        else if (taskUM.Position.HasValue && taskUM.Position != task.Position)
        {
            // Moving within the same column
            if (taskUM.Position > task.Position)
            {
                // Moving down: Shift tasks between old and new positions up
                var tasksToShift = this.context.Tasks
                    .Where(t => t.ColumnId == task.ColumnId && t.Position > task.Position && t.Position <= taskUM.Position)
                    .ToList();

                foreach (var t in tasksToShift)
                {
                    t.Position--;
                }
            }
            else
            {
                // Moving up: Shift tasks between new and old positions down
                var tasksToShift = this.context.Tasks
                    .Where(t => t.ColumnId == task.ColumnId && t.Position >= taskUM.Position && t.Position < task.Position)
                    .ToList();

                foreach (var t in tasksToShift)
                {
                    t.Position++;
                }
            }

            // Ensure the position is at least 1
            task.Position = Math.Max(taskUM.Position.Value, 1);
        }

        this.context.Tasks.Update(task);
        await this.context.SaveChangesAsync();

        return task;
    }

/*
    public async Task<DAL.Models.Task> AssignTaskAsync(TaskUM taskUM, int id)
    {
        var task = await this.GetTaskByIdAsync(id);

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
    public async Task DeleteTaskByIdAsync(int id)
    {
        var task = await this.GetTaskByIdAsync(id);

        this.context.Tasks.Remove(task);
        this.context.SaveChanges();
    }
}