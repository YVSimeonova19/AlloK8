using System.Collections.Generic;

namespace AlloK8.PL.Models;

public class KanbanVM
{
    public Dictionary<int, List<TaskVM>> TasksByColumn = new Dictionary<int, List<TaskVM>>();

    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
}