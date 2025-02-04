using Microsoft.Build.Framework;

namespace AlloK8.PL.Models;

public class ProjectVM
{
    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public int UserId { get; set; }
}