using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Labels;
using AlloK8.BLL.Common.Tasks;
using AlloK8.Common.Models.Label;
using AlloK8.PL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;

namespace AlloK8.PL.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class LabelsController : Controller
{
    private readonly ILabelService labelService;
    private readonly ITaskService taskService;

    public LabelsController(
        ILabelService labelService,
        ITaskService taskService)
    {
        this.labelService = labelService;
        this.taskService = taskService;
    }

    [HttpGet("/projects/{projectId}/labels")]
    public async Task<IActionResult> Labels(int projectId)
    {
        var labels = await this.GetProjectLabels(projectId);

        if (labels is OkObjectResult okResult)
        {
            var labelVMs = okResult.Value as List<LabelVM>;
            return this.View(labelVMs);
        }

        return this.View();
    }

    [HttpGet("/projects/{projectId}/labels/all")]
    public async Task<IActionResult> GetProjectLabels(int projectId)
    {
        try
        {
            var labels = await this.labelService.GetLabelsByProjectIdAsync(projectId);

            var labelVMs = labels.Select(label => new LabelVM
            {
                Id = label.Id,
                Title = label.Title,
                Description = label.Description,
                Color = label.Color,
            }).ToList();

            this.ViewBag.ProjectId = projectId;

            return this.Ok(labelVMs);
        }
        catch (Exception ex)
        {
            return this.BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("projects/{projectId}/new-label")]
    public async Task<IActionResult> CreateLabel(int projectId)
    {
        var model = new LabelVM();
        return this.View(model);
    }

    [HttpPost("projects/{projectId}/new-label")]
    public async Task<IActionResult> CreateLabel(int projectId, LabelVM model)
    {
        if (this.ModelState.IsValid)
        {
            var label = new LabelIM
            {
                Name = model.Title,
                Description = model.Description,
                Color = model.Color,
                ProjectId = projectId,
            };

            var result = await this.labelService.CreateLabelAsync(label);

            if (result != null)
            {
                return this.RedirectToAction("Labels", new { projectId });
            }
            else
            {
                return this.BadRequest();
            }
        }

        return this.View("CreateLabel");
    }

    [HttpDelete("labels/{labelId}/delete")]
    public async Task<IActionResult> DeleteLabel(int labelId)
    {
        try
        {
            await this.labelService.DeleteLabelAsync(labelId);
            return this.Ok();
        }
        catch (Exception ex)
        {
            return this.BadRequest(ex.Message);
        }
    }

    [HttpPost("/labels/{labelId}/edit")]
    public async Task<IActionResult> EditLabel(int labelId, [FromForm] LabelUpdateVM model)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var labelUM = new LabelUM
        {
            Title = model.Title,
            Description = model.Description,
            Color = model.Color,
        };

        var updatedLabel = await this.labelService.EditLabelAsync(labelUM, labelId);
        if (updatedLabel == null)
        {
            return this.NotFound();
        }

        return this.Ok(new
        {
            id = updatedLabel.Id,
            title = updatedLabel.Title,
            description = updatedLabel.Description,
            color = updatedLabel.Color,
        });
    }

    [HttpGet("/kanban/task/{taskId}/labels")]
    public async Task<IActionResult> GetTaskLabels(int taskId)
    {
        try
        {
            var labels = await this.labelService.GetLabelsByTaskIdAsync(taskId);

            var labelVMs = labels.Select(label => new LabelVM
            {
                Id = label.Id,
                Title = label.Title,
                Description = label.Description,
                Color = label.Color,
            }).ToList();

            return this.Ok(labelVMs);
        }
        catch (Exception ex)
        {
            return this.BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("/kanban/task/{taskId}/labels/{labelId}/remove")]
    public async Task<IActionResult> RemoveLabelFromTask(int taskId, int labelId)
    {
        try
        {
            await this.taskService.RemoveLabelFromTaskAsync(taskId, labelId);
            return this.Ok();
        }
        catch (Exception ex)
        {
            return this.BadRequest($"Error: {ex.Message}");
        }
    }
}