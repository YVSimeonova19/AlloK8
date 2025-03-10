using System.Collections.Generic;

namespace AlloK8.PL.Models;

public class KanbanVM
{
    public Dictionary<int, List<TaskKanbanVM>> TasksByColumn = new Dictionary<int, List<TaskKanbanVM>>();

    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
}