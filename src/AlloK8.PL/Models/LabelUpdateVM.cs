using System.ComponentModel.DataAnnotations;

namespace AlloK8.PL.Models;

public class LabelUpdateVM
{
    public int Id { get; set; }

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

    [StringLength(
        7,
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "HEX7ErrorMessage")]
    public string? Color { get; set; }
}