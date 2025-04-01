using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AlloK8.DAL.Models;

namespace AlloK8.PL.Models;

public class ProjectUpdateVM
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

    public List<UserProfile> Users { get; set; } = [];

    public List<UserProfile> AllUsers { get; set; } = [];
}