using System;

namespace AlloK8.BLL.Common.Tasks;

public class TaskUM
{
    public string? Title { get; set; } = null;
    public string? Description { get; set; } = null;
    public bool? IsPriority { get; set; } = null;
    public int? Position { get; set; } = null;
    public DateTime? StartDate { get; set; } = null;
    public DateTime? DueDate { get; set; } = null;
    public int? ColumnId { get; set; } = null;
}