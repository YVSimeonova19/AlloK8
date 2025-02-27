using System;
using System.Collections.Generic;

namespace AlloK8.DAL.Models;

public class Column : Entity
{
    public string? Name { get; set; }
    public int Position { get; set; }

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ICollection<Task> Tasks { get; } = new List<Task>();
}