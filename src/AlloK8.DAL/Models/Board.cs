using System;
using System.Collections.Generic;

namespace AlloK8.DAL.Models;

public class Board : AuditableEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ICollection<Column> Columns { get; } = new List<Column>();

    public List<UserProfile> Users { get; } = [];
}