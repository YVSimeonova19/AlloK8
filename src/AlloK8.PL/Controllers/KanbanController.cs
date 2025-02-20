using System.Linq;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Tasks;
using AlloK8.PL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlloK8.PL.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class KanbanController : Controller
{
    private readonly ITaskService taskService;

    public KanbanController(ITaskService taskService)
    {
        this.taskService = taskService;
    }

    [HttpGet("/kanban")]
    public async Task<IActionResult> Kanban()
    {
        var tasks = await this.taskService.GetAllTasks();
        var taskVMs = tasks
            .Select(t => new TaskVM
            {
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