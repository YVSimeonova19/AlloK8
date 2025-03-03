using System;
using System.Collections.Generic;

namespace AlloK8.DAL.Models;

public class Column : Entity
{
    public string? Name { get; set; }
    public int Position { get; set; }

    public ICollection<Task> Tasks { get; } = new List<Task>();
}