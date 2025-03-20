using System.Collections.Generic;

namespace AlloK8.DAL.Models;

public class Label : Entity
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }

    public List<DAL.Models.Task> Tasks { get; } = [];
}