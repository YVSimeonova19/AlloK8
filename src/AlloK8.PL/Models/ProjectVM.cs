using Microsoft.Build.Framework;

namespace AlloK8.PL.Models;

public class ProjectVM
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public int UserId { get; set; }
}