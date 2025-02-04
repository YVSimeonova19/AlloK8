using System.Collections.Generic;
using AlloK8.BLL.Common.Projects;
using AlloK8.PL.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlloK8.PL.Controllers;

public class ProjectsController : Controller
{
    private readonly IProjectService projectService;

    public ProjectsController(IProjectService projectService)
    {
        this.projectService = projectService;
    }

    [HttpGet("/projects")]
    public IActionResult Projects()
    {
        var projects = this.projectService.GetAllProjects();
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
}