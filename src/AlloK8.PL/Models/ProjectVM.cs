using System.ComponentModel.DataAnnotations;

namespace AlloK8.PL.Models;

public class ProjectVM
{
    public int Id { get; set; }

    [Required]
    [StringLength(
        30,
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "Title30ErrorMessage")]
    public string? Name { get; set; }

    [Required]
    [StringLength(
        100,
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "Description100ErrorMessage")]
    public string? Description { get; set; }

    [Required]
    public int UserId { get; set; }
}