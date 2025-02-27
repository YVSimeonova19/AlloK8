using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Tasks;
using AlloK8.BLL.Common.Users;
using AlloK8.BLL.Identity.Contracts;
using AlloK8.PL.Models;
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

    public KanbanController(
        ITaskService taskService,
        ICurrentUser currentUser,
        IUserService userService)
    {
        this.taskService = taskService;
        this.currentUser = currentUser;
        this.userService = userService;
    }

    [HttpGet("/kanban")]
    public async Task<IActionResult> Kanban()
    {
        var tasks = await this.taskService.GetAllTasks();
        var taskVMs = tasks
            .Select(t => new TaskVM
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Position = t.Position,
                ColumnId = t.ColumnId,
            })
            .ToList();

        var tasksByColumn = taskVMs
            .GroupBy(t => t.ColumnId)
            .ToDictionary(g => g.Key, g => g.ToList());

        return this.View(tasksByColumn);
    }

    [HttpPost("/kanban/create")]
    public async Task<IActionResult> CreateTask(string title)
    {
        var taskIM = new TaskIM
        {
            Title = title,
            CreatorId = (await this.userService.GetUserProfileByGuid(this.currentUser.UserId)).Id,
            CreatedOn = DateTime.Now,
        };

        await this.taskService.CreateTask(taskIM);

        return this.RedirectToAction("Kanban");
    }

    [HttpPost("/kanban/move")]
    public async Task<IActionResult> MoveTask(int id, int columnId, int position)
    {
        var taskUM = new TaskUM
        {
            ColumnId = columnId,
            Position = position,
        };

        await this.taskService.MoveTask(taskUM, id);

        return this.RedirectToAction("Kanban");
    }

    [HttpPost("/kanban/delete")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        await this.taskService.DeleteTaskById(id);

        return this.RedirectToAction("Kanban");
    }
}