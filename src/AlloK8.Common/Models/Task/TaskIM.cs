using System;
using System.ComponentModel.DataAnnotations;

namespace AlloK8.BLL.Common.Tasks;

public class TaskIM
{
    [Required]
    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    [Required]
    public int CreatorId { get; set; }

    [Required]
    public DateTime? CreatedOn { get; set; }

    public int? ColumnId { get; set; }

    // Add assignees
}