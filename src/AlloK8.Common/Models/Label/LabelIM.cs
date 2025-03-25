using System.ComponentModel.DataAnnotations;

namespace AlloK8.Common.Models.Label;

public class LabelIM
{
    [Required]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Required]
    public string? Color { get; set; }

    [Required]
    public int ProjectId { get; set; }
}