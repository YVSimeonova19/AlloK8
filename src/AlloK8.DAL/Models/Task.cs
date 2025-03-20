using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlloK8.DAL.Models;

public class Task : AuditableEntity
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsPriority { get; set; } = false;

    public int Position { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }

    public int ColumnId { get; set; }
    public Column Column { get; set; } = null!;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public List<UserProfile> Assignees { get; } = [];

    public List<Label> Labels { get; } = [];
}