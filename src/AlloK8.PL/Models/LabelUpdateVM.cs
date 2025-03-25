using System.ComponentModel.DataAnnotations;

namespace AlloK8.PL.Models;

public class LabelUpdateVM
{
    public int Id { get; set; }

    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string? Title { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [StringLength(7, ErrorMessage = "Color HEX cannot exceed 6 characters.")]
    public string? Color { get; set; }
}