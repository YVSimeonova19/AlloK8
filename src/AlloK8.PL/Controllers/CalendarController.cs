using System.Linq;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Projects;
using AlloK8.BLL.Common.Tasks;
using AlloK8.PL.Models;
using AlloK8.PL.Models.Requests;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlloK8.PL.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class CalendarController : Controller
{
    private readonly ITaskService taskService;
    private readonly IProjectService projectService;

    public CalendarController(
        ITaskService taskService,
        IProjectService projectService)
    {
        this.taskService = taskService;
        this.projectService = projectService;
    }

    [HttpGet("/projects/{projectId}/calendar")]
    public async Task<IActionResult> Calendar(int projectId)
    {
        var tasks = await this.taskService.GetAllTasksByProjectIdAsync(projectId);

        var calendarVM = new CalendarVM()
        {
            ProjectId = projectId,
            ProjectName = (await this.projectService.GetProjectByIdAsync(projectId)).Name,
        };

        var taskVMs = tasks
            .Select(t => new TaskCalendarVM
            {
                Id = t.Id,
                title = t.Title,
                start = t.StartDate,
                end = t.DueDate,
            })
            .ToList();

        calendarVM.Tasks = taskVMs;

        return this.View(calendarVM);
    }
}