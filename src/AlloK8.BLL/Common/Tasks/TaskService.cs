using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Users;
using AlloK8.DAL;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace AlloK8.BLL.Common.Tasks;

internal class TaskService : ITaskService
{
    private readonly EntityContext context;
    private readonly IUserService userService;

    public TaskService(
        EntityContext context,
        IUserService userService)
    {
        this.context = context;
        this.userService = userService;
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
        await this.context.SaveChangesAsync();

        return task;
    }

    public async Task<DAL.Models.Task> GetTaskByIdAsync(int id)
    {
        var task = await this.context.Tasks
            .Where(t => t.Id == id)
            .Include(t => t.Assignees)
            .ThenInclude(a => a.ApplicationUser)
            .FirstOrDefaultAsync();

        if (task == null)
        {
            throw new KeyNotFoundException("Task not found");
        }

        return task;
    }

    public async Task<List<DAL.Models.Task>> GetAllTasksAsync()
    {
        return await this.context.Tasks.ToListAsync();
    }

    public async Task<List<DAL.Models.Task>> GetAllTasksByProjectIdAsync(int projectId)
    {
        return await this.context.Tasks
            .Where(t => t.Project.Id == projectId)
            .ToListAsync();
    }

    public async Task<DAL.Models.Task> UpdateTaskAsync(TaskUM taskUM, int id)
    {
        var task = await this.GetTaskByIdAsync(id);

        task.Title = taskUM.Title ?? task.Title;
        task.Description = taskUM.Description ?? task.Description;
        task.IsPriority = taskUM.IsPriority ?? task.IsPriority;
        task.StartDate = taskUM.StartDate ?? task.StartDate;
        task.DueDate = taskUM.DueDate ?? task.DueDate;
        task.Position = taskUM.Position ?? task.Position;

        this.context.Tasks.Update(task);
        await this.context.SaveChangesAsync();

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
            var tasksInNewColumn = await this.context.Tasks
                .Where(t => t.ColumnId == taskUM.ColumnId && t.Position >= newPosition)
                .ToListAsync();

            foreach (var t in tasksInNewColumn)
            {
                t.Position++;
            }

            // Shift positions in the old column to fill the gap
            var tasksInOldColumn = await this.context.Tasks
                .Where(t => t.ColumnId == task.ColumnId && t.Position > task.Position)
                .ToListAsync();

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
                var tasksToShift = await this.context.Tasks
                    .Where(t => t.ColumnId == task.ColumnId && t.Position > task.Position && t.Position <= taskUM.Position)
                    .ToListAsync();

                foreach (var t in tasksToShift)
                {
                    t.Position--;
                }
            }
            else
            {
                // Moving up: Shift tasks between new and old positions down
                var tasksToShift = await this.context.Tasks
                    .Where(t => t.ColumnId == task.ColumnId && t.Position >= taskUM.Position && t.Position < task.Position)
                    .ToListAsync();

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

    public async Task AssignTaskAsync(int taskId, int userId)
    {
        var task = await this.GetTaskByIdAsync(taskId);
        var user = await this.userService.GetUserProfileByIdAsync(userId);

        if (task.Assignees.Any(u => u.Id == user.Id))
        {
            throw new Exception("User is already assigned to this task.");
        }

        task.Assignees.Add(user);
        this.context.Update(task);
        await this.context.SaveChangesAsync();
    }

    public async Task RemoveUserFromTaskAsync(int taskId, int userId)
    {
        var task = await this.GetTaskByIdAsync(taskId);
        var user = await this.userService.GetUserProfileByIdAsync(userId);

        if (task.Assignees.Contains(user))
        {
            task.Assignees.Remove(user);
            this.context.Update(task);
            await this.context.SaveChangesAsync();
        }
        else
        {
            throw new Exception("User is not assigned to this task.");
        }
    }

    public async Task DeleteTaskByIdAsync(int id)
    {
        var task = await this.GetTaskByIdAsync(id);

        this.context.Tasks.Remove(task);
        await this.context.SaveChangesAsync();
    }

    public async Task<List<DAL.Models.Task>> PrioritizeTasksAsync(List<DAL.Models.Task> tasks)
    {
        // Create a priority queue with a custom comparer
        var comparer = Comparer<bool>.Create((x, y) => y.CompareTo(x));
        var priorityQueue = new PriorityQueue<DAL.Models.Task, bool>(comparer);

        // Enqueue all tasks
        foreach (var task in tasks)
        {
            priorityQueue.Enqueue(task, task.IsPriority);
        }

        // Dequeue tasks in priority order
        List<DAL.Models.Task> prioritizedTasks = new List<DAL.Models.Task>();
        while (priorityQueue.Count > 0)
        {
            prioritizedTasks.Add(priorityQueue.Dequeue());
        }

        return prioritizedTasks;
    }
}