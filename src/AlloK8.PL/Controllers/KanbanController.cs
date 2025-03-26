using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Labels;
using AlloK8.BLL.Common.Projects;
using AlloK8.BLL.Common.Search;
using AlloK8.BLL.Common.Tasks;
using AlloK8.BLL.Common.Users;
using AlloK8.BLL.Identity.Contracts;
using AlloK8.PL.Models;
using AlloK8.PL.Models.Requests;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task = AlloK8.DAL.Models.Task;

namespace AlloK8.PL.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class KanbanController : Controller
{
    private readonly ITaskService taskService;
    private readonly ICurrentUser currentUser;
    private readonly IUserService userService;
    private readonly IProjectService projectService;
    private readonly ISearchService searchService;
    private readonly ILabelService labelService;

    public KanbanController(
        ITaskService taskService,
        ICurrentUser currentUser,
        IUserService userService,
        IProjectService projectService,
        ISearchService searchService,
        ILabelService labelService)
    {
        this.taskService = taskService;
        this.currentUser = currentUser;
        this.userService = userService;
        this.projectService = projectService;
        this.searchService = searchService;
        this.labelService = labelService;
    }

    [HttpGet("/projects/{projectId}/kanban")]
    public async Task<IActionResult> Kanban(int projectId, bool sortByPriority = false, bool onlyMine = false)
    {
        var tasks = new List<Task>();

        if (onlyMine)
        {
            var assignee = this.userService.GetUserProfileByGuidAsync(this.currentUser.UserId).Result;
            tasks = await this.taskService.GetAllTasksByAssigneeIdAsync(projectId, assignee.Id);
        }
        else
        {
            tasks = await this.taskService.GetAllTasksByProjectIdAsync(projectId);
        }

        if (sortByPriority)
        {
            tasks = await this.taskService.PrioritizeTasksAsync(tasks);
        }
        else
        {
            tasks = tasks.OrderBy(t => t.Position).ToList();
        }

        var kanbanVM = new KanbanVM
        {
            ProjectId = projectId,
            ProjectName = (await this.projectService.GetProjectByIdAsync(projectId)).Name,
        };

        var taskVMs = tasks
            .Select(t => new TaskKanbanVM
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsPriority = t.IsPriority,
                Position = t.Position,
                ColumnId = t.ColumnId,
            })
            .ToList();

        kanbanVM.TasksByColumn = taskVMs
            .GroupBy(t => t.ColumnId)
            .ToDictionary(g => g.Key, g => g.ToList());

        return this.View(kanbanVM);
    }

    [HttpPost("/kanban/create")]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Title))
        {
            return this.BadRequest(new { success = false, message = "Invalid task title" });
        }

        var taskIM = new TaskIM
        {
            Title = request.Title,
            CreatorId = (await this.userService.GetUserProfileByGuidAsync(this.currentUser.UserId)).Id,
            CreatedOn = DateTime.UtcNow,
            ColumnId = request.ColumnId,
            ProjectId = request.ProjectId,
        };

        var createdTask = await this.taskService.CreateTaskAsync(taskIM);

        return this.Ok(new { success = true, id = createdTask.Id, title = createdTask.Title });
    }

    [HttpPost("/kanban/move")]
    public async Task<IActionResult> MoveTask([FromBody] MoveTaskRequest request)
    {
        if (request == null || request.Id <= 0 || request.ColumnId <= 0 || request.Position < 0)
        {
            return this.BadRequest(new { success = false, message = "Invalid task data" });
        }

        var taskUM = new TaskUM
        {
            ColumnId = request.ColumnId,
            Position = request.Position,
        };

        try
        {
            await this.taskService.MoveTaskAsync(taskUM, request.Id);
            return this.Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return this.StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpPost("/kanban/delete")]
    public async Task<IActionResult> DeleteTask([FromBody] DeleteTaskRequest request)
    {
        if (request == null || request.Id <= 0)
        {
            return this.BadRequest(new { success = false, message = "Invalid task ID" });
        }

        try
        {
            await this.taskService.DeleteTaskByIdAsync(request.Id);
            return this.Ok(new { success = true });
        }
        catch (Exception ex)
        {
            return this.StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpGet("kanban/task/{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
        var task = await this.taskService.GetTaskByIdAsync(id);
        if (task == null)
        {
            return this.NotFound();
        }

        var taskViewModel = new
        {
            id = task.Id,
            title = task.Title,
            description = task.Description,
            startDate = task.StartDate,
            dueDate = task.DueDate,
            isPriority = task.IsPriority,
            columnId = task.ColumnId,
            position = task.Position,
            users = task.Assignees.Select(u => new
            {
                id = u.Id,
                applicationUser = new
                {
                    email = u.ApplicationUser!.Email,
                },
            }).ToList(),
        };

        return this.Ok(taskViewModel);
    }

    [HttpPost("kanban/edit-task")]
    public async Task<IActionResult> EditTask([FromForm] TaskUpdateVM model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var taskUM = new TaskUM
        {
            Title = model.Title,
            Description = model.Description,
            IsPriority = model.IsPriority,
            StartDate = model.StartDate,
            DueDate = model.DueDate,
        };

        var updatedTask = await this.taskService.UpdateTaskAsync(taskUM, model.Id);
        if (updatedTask == null)
        {
            return this.NotFound();
        }

        try
        {
            if (model.Users != null)
            {
                foreach (var user in model.Users)
                {
                    await this.taskService.AssignTaskAsync(model.Id, user.Id);
                }
            }

            if (model.LabelIds != null && model.LabelIds.Any())
            {
                var currentLabels = await this.labelService.GetLabelsByTaskIdAsync(model.Id);
                foreach (var label in currentLabels)
                {
                    if (!model.LabelIds.Contains(label.Id))
                    {
                        await this.taskService.RemoveLabelFromTaskAsync(model.Id, label.Id);
                    }
                }

                foreach (var labelId in model.LabelIds)
                {
                    await this.taskService.AddLabelToTaskAsync(model.Id, labelId);
                }
            }

            var refreshedTask = await this.taskService.GetTaskByIdAsync(model.Id);

            var responseDto = new
            {
                id = refreshedTask.Id,
                title = refreshedTask.Title,
                description = refreshedTask.Description,
                startDate = refreshedTask.StartDate,
                dueDate = refreshedTask.DueDate,
                isPriority = refreshedTask.IsPriority,
                columnId = refreshedTask.ColumnId,
                position = refreshedTask.Position,
                users = refreshedTask.Assignees?.Select(u => new
                {
                    id = u.Id,
                    applicationUser = new
                    {
                        email = u.ApplicationUser!.Email,
                    },
                }).ToList(),
                labels = refreshedTask.Labels?.Select(l => new
                {
                    id = l.Id,
                    title = l.Title,
                    color = l.Color,
                }).ToList(),
            };

            return this.Ok(responseDto);
        }
        catch (Exception ex)
        {
            return this.BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpPost("kanban/assign-user")]
    public async Task<IActionResult> AssignUserToTask([FromBody] AssignUserRequest request)
    {
        if (request == null || request.TaskId <= 0 || request.UserId <= 0)
        {
            return this.BadRequest(new { success = false, message = "Invalid request data" });
        }

        try
        {
            await this.taskService.AssignTaskAsync(request.TaskId, request.UserId);
            return this.Ok();
        }
        catch (Exception ex)
        {
            return this.StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpPost("/kanban/assign-label")]
    public async Task<IActionResult> AssignLabelToTask([FromBody] AssignLabelRequest request)
    {
        if (request == null || request.TaskId <= 0 || request.LabelId <= 0)
        {
            return this.BadRequest(new { success = false, message = "Invalid request data" });
        }

        try
        {
            await this.taskService.AddLabelToTaskAsync(request.TaskId, request.LabelId);
            return this.Ok();
        }
        catch (Exception ex)
        {
            return this.StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    [HttpPost("/kanban/{taskId}/remove-user/{userId}")]
    public async Task<IActionResult> RemoveUserFromTask(int taskId, int userId)
    {
        try
        {
            await this.taskService.RemoveUserFromTaskAsync(taskId, userId);
            return this.Ok();
        }
        catch (Exception ex)
        {
            return this.BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpGet("kanban/users/search")]
    public async Task<IActionResult> SearchUsersByEmail([FromQuery] string email, int projectId)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return this.BadRequest("Email cannot be empty.");
        }

        var users = await this.searchService.SearchTaskUsersByEmailAsync(email, projectId);
        return this.Ok(users);
    }
}