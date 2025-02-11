using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Projects;
using AlloK8.BLL.Common.Users;
using AlloK8.BLL.Identity.Contracts;
using AlloK8.Common.Models.Project;
using AlloK8.PL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlloK8.PL.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class ProjectsController : Controller
{
    private readonly IProjectService projectService;
    private readonly ICurrentUser currentUser;
    private readonly IUserService userService;

    public ProjectsController(
        IProjectService projectService,
        ICurrentUser currentUser,
        IUserService userService)
    {
        this.projectService = projectService;
        this.currentUser = currentUser;
        this.userService = userService;
    }

    [HttpGet("/projects")]
    public async Task<IActionResult> Projects()
    {
        var projects = await this.projectService.GetProjectsByUserId(this.currentUser.UserId);
        var projectModels = new List<ProjectVM>();

        foreach (var project in projects)
        {
            var name = project.Name;
            var decription = project.Description;

            projectModels.Add(new ProjectVM()
            {
                Name = name,
                Description = decription,
            });
        }

        return this.View(projectModels);
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
                CreatorId = this.userService.GetUserProfileByGuid(this.currentUser.UserId).Id,
                CreatedOn = DateTime.Now,
            };

            var result = this.projectService.CreateProject(project);

            if (result != null)
            {
                return this.RedirectToAction("Projects");
            }
            else
            {
                return this.BadRequest();
            }
        }

        return this.View("CreateProject");
    }
}