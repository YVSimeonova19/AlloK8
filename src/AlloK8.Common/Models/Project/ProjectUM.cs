using System;
using System.ComponentModel.DataAnnotations;

namespace AlloK8.Common.Models.Project;

public class ProjectUM
{
    public string? Title { get; set; } = null;
    public string? Description { get; set; } = null;

    [Required]
    public int UpdatorId { get; set; }

    [Required]
    public DateTime? UpdatedOn { get; set; }
}