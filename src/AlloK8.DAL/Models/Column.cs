using System;
using System.Collections.Generic;

namespace AlloK8.DAL.Models;

public class Column : Entity
{
    public string? Name { get; set; }
    public int Position { get; set; }

    public int BoardId { get; set; }
    public Board Board { get; set; } = null!;

    public ICollection<Task> Tasks { get; } = new List<Task>();
}