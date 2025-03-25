using System;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Labels;
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

    public LabelsController(ILabelService labelService)
    {
        this.labelService = labelService;
    }

    [HttpGet("/projects/{projectId}/labels")]
    public async Task<IActionResult> Labels(int projectId)
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

            return this.View(labelVMs);
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
}