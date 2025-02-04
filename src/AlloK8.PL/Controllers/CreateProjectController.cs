using System;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Projects;
using AlloK8.BLL.Identity.Contracts;
using AlloK8.Common.Models.Project;
using AlloK8.PL.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlloK8.PL.Controllers;

public class CreateProjectController : Controller
{
    private readonly IProjectService projectService;
    private readonly ICurrentUser currentUser;

    public CreateProjectController(
        IProjectService projectService,
        ICurrentUser currentUser)
    {
        this.projectService = projectService;
        this.currentUser = currentUser;
    }

    [HttpGet("/new-project")]
    public async Task<IActionResult> CreateProject()
    {
        var model = new ProjectVM();
        return this.View(model);
    }

    [HttpPost("/new-project")]
    public async Task<IActionResult> CreateProject(ProjectVM model)
    {
        if (this.ModelState.IsValid)
        {
            var project = new ProjectIM
            {
                Name = model.Name,
                Description = model.Description,
                CreatorId = 1,
                //CreatorId = this.currentUser.UserId,
                CreatedOn = DateTime.Now,
            };

            var result = this.projectService.CreateProject(project);

            if (result != null)
            {
                return this.Ok();
            }
            else
            {
                return this.BadRequest();
            }
        }

        return this.View(model);
    }
}