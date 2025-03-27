using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Invoices;
using AlloK8.BLL.Common.Projects;
using AlloK8.BLL.Common.Search;
using AlloK8.BLL.Common.Users;
using AlloK8.BLL.Identity.Contracts;
using AlloK8.Common.Models.Project;
using AlloK8.PL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using PointF = Syncfusion.Drawing.PointF;
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

    [HttpGet("/project/{projectId}/create-report")]
    public async Task<IActionResult> CreateReport([FromRoute] int projectId)
    {
        try
        {
            // Create data source
            var invoiceDetails = await this.reportService.GetProjectProgressAsync(projectId);

            // Setup pdf document
            var document = new PdfDocument();
            document.PageSettings.Size = PdfPageSize.A4;
            document.PageSettings.Orientation = PdfPageOrientation.Landscape;
            var page = document.Pages.Add();
            var graphics = page.Graphics;
            var grid = new PdfGrid
            {
                // Add the data source
                DataSource = invoiceDetails,
            };
            var primaryColor = new PdfColor(75, 73, 172);
            var font = new PdfTrueTypeFont("Arial Unicode MS", 14);

            // Create the grid cell styles
            var cellStyle = new PdfGridCellStyle
            {
                Borders =
                {
                    All = new PdfPen(primaryColor),
                },
                TextBrush = PdfBrushes.Black,
            };

            var header = grid.Headers[0];

            // Create the header style
            var headerStyle = new PdfGridCellStyle
            {
                Borders =
                {
                    All = new PdfPen(primaryColor),
                },
                BackgroundBrush = new PdfSolidBrush(primaryColor),
                TextBrush = PdfBrushes.White,
            };

            // Apply the header style
            header.ApplyStyle(headerStyle);

            // Create the layout format for grid
            var layoutFormat = new PdfGridLayoutFormat
            {
                // Allow table pagination
                Layout = PdfLayoutType.Paginate,
            };

            // Draw the grid to the PDF page
            grid.Draw(
                page,
                new PointF(10, 10),
                layoutFormat);

            // Save the document to memory stream
            using var stream = new MemoryStream();
            document.Save(stream);
            document.Close(true);

            // Reset the position to the beginning of the stream
            stream.Position = 0;

            // Return the PDF as a file
            return this.File(
                fileContents: stream.ToArray(),
                contentType: "application/pdf",
                fileDownloadName: $"Project{projectId}Report.pdf",
                enableRangeProcessing: false);
        }
        catch (Exception ex)
        {
            return this.BadRequest($"Error: {ex.Message}");
        }
    }
}