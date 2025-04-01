using System.ComponentModel.DataAnnotations;

namespace AlloK8.PL.Models;

public class LabelVM
{
    public int Id { get; set; }

    [Required]
    [StringLength(
        30,
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "Title30ErrorMessage")]
    public string? Title { get; set; }

    [Required]
    [StringLength(
        100,
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "Description100ErrorMessage")]
    public string? Description { get; set; }

    [Required]
    [StringLength(
        7,
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "HEX7ErrorMessage")]
    public string? Color { get; set; } = string.Empty;
}