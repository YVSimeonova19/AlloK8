using Microsoft.Build.Framework;

namespace AlloK8.PL.Models;

public class LabelVM
{
    public int Id { get; set; }

    [Required]
    public string? Title { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public string? Color { get; set; } = string.Empty;
}