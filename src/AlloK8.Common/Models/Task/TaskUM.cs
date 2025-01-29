using System;

namespace AlloK8.BLL.Common.Tasks;

public class TaskUM
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Position { get; set; }
    public DateTime? DueDate { get; set; }
    public int? ColumnId { get; set; }
}