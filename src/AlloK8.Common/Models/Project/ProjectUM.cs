using System;
using System.ComponentModel.DataAnnotations;

namespace AlloK8.Common.Models.Project;

public class ProjectUM
{
    public string? Title { get; set; }
    public string? Description { get; set; }

    [Required]
    public int UpdatorId { get; set; }

    [Required]
    public DateTime? UpdatedOn { get; set; }
}