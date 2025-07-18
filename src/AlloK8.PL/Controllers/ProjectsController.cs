using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Projects;
using AlloK8.BLL.Common.Reports;
using AlloK8.BLL.Common.Search;
using AlloK8.BLL.Common.Users;
using AlloK8.BLL.Identity.Contracts;
using AlloK8.Common.Models.Project;
using AlloK8.PL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectVM = AlloK8.PL.Models.ProjectVM;

namespace AlloK8.PL.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class ProjectsController : Controller
{
    private readonly IProjectService projectService;
    private readonly ICurrentUser currentUser;
    private readonly IUserService userService;
    private readonly ISearchService searchService;
    private readonly IReportService reportService;

    public ProjectsController(
        IProjectService projectService,
        ICurrentUser currentUser,
        IUserService userService,
        ISearchService searchService,
        IReportService reportService)
    {
        this.projectService = projectService;
        this.currentUser = currentUser;
        this.userService = userService;
        this.searchService = searchService;
        this.reportService = reportService;
    }

    [HttpGet("/projects")]
    public async Task<IActionResult> Projects()
    {
        var projects = await this.projectService.GetProjectsByUserIdAsync(this.currentUser.UserId);
        var projectModels = new List<ProjectVM>();

        foreach (var project in projects)
        {
            var id = project.Id;
            var name = project.Name;
            var decription = project.Description;

            projectModels.Add(new ProjectVM()
            {
                Id = id,
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
                CreatorId = (await this.userService.GetUserProfileByGuidAsync(this.currentUser.UserId)).Id,
                CreatedOn = DateTime.UtcNow,
            };

            var result = await this.projectService.CreateProjectAsync(project);

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

    [HttpPost("projects/edit-project")]
    public async Task<IActionResult> EditProject([FromForm] ProjectUpdateVM model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var projectUM = new ProjectUM
        {
            Title = model.Title,
            Description = model.Description,
        };

        var updatedProject = await this.projectService.UpdateProjectAsync(projectUM, model.Id);
        if (updatedProject == null)
        {
            return this.NotFound();
        }

        return this.Ok(new
        {
            id = updatedProject.Id,
            title = updatedProject.Name,
            description = updatedProject.Description,
        });
    }

    [HttpDelete("/projects/{id}/delete")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            await this.projectService.DeleteProjectByIdAsync(id);
            return this.Ok();
        }
        catch (Exception ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpPost("/projects/add-users")]
    public async Task<IActionResult> AddUsers([FromBody] ProjectUpdateVM model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest("Invalid request.");
        }

        try
        {
            foreach (var user in model.Users)
            {
                await this.projectService.AddUserToProjectAsync(model.Id, user.Id);
            }

            return this.Ok();
        }
        catch (Exception ex)
        {
            return this.BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpPost("/projects/{projectId}/remove-user/{userId}")]
    public async Task<IActionResult> RemoveUserFromProject(int projectId, int userId)
    {
        try
        {
            await this.projectService.RemoveUserFromProjectAsync(projectId, userId);
            return this.Ok();
        }
        catch (Exception ex)
        {
            return this.BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpGet("/projects/{projectId}/users")]
    public async Task<IActionResult> GetProjectUsers(int projectId)
    {
        var users = await this.projectService.GetAllUsersByProjectIdAsync(projectId);
        return this.Ok(users);
    }

    [HttpGet("api/users/search")]
    public async Task<IActionResult> SearchUsersByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return this.BadRequest("Email cannot be empty.");
        }

        var users = await this.searchService.SearchUsersByEmailAsync(email);
        return this.Ok(users);
    }

    [HttpGet("/project/{projectId}/{projectName}/create-report")]
    public async Task<IActionResult> CreateReport([FromRoute] int projectId, [FromRoute] string projectName)
    {
        try
        {
            var pdfBytes = await this.reportService.GenerateProjectReportPdfAsync(projectId, projectName);
            return this.File(pdfBytes, "application/pdf", $"{projectName}_Report.pdf");
        }
        catch (Exception ex)
        {
            return this.BadRequest($"Error: {ex.Message}");
        }
    }
}