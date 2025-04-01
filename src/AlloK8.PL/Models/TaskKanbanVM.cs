using System.ComponentModel.DataAnnotations;

namespace AlloK8.PL.Models;

public class TaskKanbanVM
{
    [Microsoft.Build.Framework.Required]
    public int Id { get; set; }

    [Required]
    [StringLength(
        30,
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "Title30ErrorMessage")]
    public string? Title { get; set; }

    [StringLength(
        100,
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "Description100ErrorMessage")]
    public string? Description { get; set; }

    public bool IsPriority { get; set; }

    [Required]
    public int Position { get; set; }

    [Required]
    public int ColumnId { get; set; }
}