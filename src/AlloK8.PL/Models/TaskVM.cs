using Microsoft.Build.Framework;

namespace AlloK8.PL.Models;

public class TaskVM
{
    [Required]
    public string? Title { get; set; }

    public string? Description { get; set; }

    [Required]
    public int Position { get; set; }

    [Required]
    public int ColumnId { get; set; }
}