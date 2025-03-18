using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AlloK8.DAL.Models;

namespace AlloK8.PL.Models;

public class ProjectUpdateVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string? Title { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    public List<UserProfile> Users { get; set; } = [];

    public List<UserProfile> AllUsers { get; set; } = [];
}