using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlloK8.Common.Models.Label;
using AlloK8.DAL;
using AlloK8.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace AlloK8.BLL.Common.Labels;

public class LabelService : ILabelService
{
    private readonly EntityContext context;

    public LabelService(EntityContext context)
    {
        this.context = context;
    }

    public async Task<Label> CreateLabelAsync(LabelIM labelIM)
    {
        var label = new Label
        {
            Title = labelIM.Name,
            Description = labelIM.Description,
            Color = labelIM.Color,
            ProjectId = labelIM.ProjectId,
        };

        this.context.Labels.Add(label);
        await this.context.SaveChangesAsync();

        return label;
    }

    public async Task<Label> GetLabelByIdAsync(int id)
    {
        var label = await this.context.Labels.FindAsync(id);

        if (label == null)
        {
            throw new KeyNotFoundException();
        }

        return label;
    }

    public async Task<List<Label>> GetLabelsByTaskIdAsync(int taskId)
    {
        return await this.context.Labels
            .Where(l => l.Tasks.Any(t => t.Id == taskId))
            .ToListAsync();
    }

    public async Task<List<Label>> GetLabelsByProjectIdAsync(int projectId)
    {
        return await this.context.Labels
            .Where(l => l.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<Label> EditLabelAsync(LabelUM labelUM, int labelId)
    {
        var label = await this.GetLabelByIdAsync(labelId);

        label.Title = labelUM.Title ?? labelUM.Title;
        label.Description = labelUM.Description ?? labelUM.Description;
        label.Color = labelUM.Color ?? labelUM.Color;

        this.context.Labels.Update(label);
        await this.context.SaveChangesAsync();

        return label;
    }

    public async Task DeleteLabelAsync(int id)
    {
        var label = await this.GetLabelByIdAsync(id);
        this.context.Labels.Remove(label);
        await this.context.SaveChangesAsync();
    }
}