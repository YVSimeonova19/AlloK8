using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Projects;
using AlloK8.BLL.Common.Tasks;
using AlloK8.BLL.Common.Users;
using AlloK8.BLL.Identity.Contracts;
using AlloK8.PL.Models;
using AlloK8.PL.Models.Requests;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlloK8.PL.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class KanbanController : Controller
{
    private readonly ITaskService taskService;
    private readonly ICurrentUser currentUser;
    private readonly IUserService userService;
    private readonly IProjectService projectService;

    public KanbanController(
        ITaskService taskService,
        ICurrentUser currentUser,
        IUserService userService,
        IProjectService projectService)
    {
        this.taskService = taskService;
        this.currentUser = currentUser;
        this.userService = userService;
        this.projectService = projectService;
    }

    [HttpGet("/projects/{projectId}/kanban")]
    public async Task<IActionResult> Kanban(int projectId)
    {
        var tasks = await this.taskService.GetAllTasksByProjectIdAsync(projectId);

        var kanbanVM = new KanbanVM
        {
            ProjectId = projectId,
            ProjectName = (await this.projectService.GetProjectByIdAsync(projectId)).Name,
        };

        var taskVMs = tasks
            .OrderBy(t => t.Position)
            .Select(t => new TaskVM
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
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
            CreatedOn = DateTime.Now,
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
}