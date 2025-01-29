using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlloK8.DAL.Models;

public class Task : AuditableEntity
{
    public string? Title { get; set; }
    public string? Description { get; set; }

    public int Position { get; set; }

    // to remove start date
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }

    public int ColumnId { get; set; }
    public Column Column { get; set; } = null!;

    public List<UserProfile> Assignees { get; } = [];
}