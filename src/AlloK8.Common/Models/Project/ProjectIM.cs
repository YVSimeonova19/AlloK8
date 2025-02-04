using System;
using System.ComponentModel.DataAnnotations;

namespace AlloK8.Common.Models.Project;

public class ProjectIM
{
    [Required]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Required]
    public int CreatorId { get; set; }

    [Required]
    public DateTime? CreatedOn { get; set; }
}