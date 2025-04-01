using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AlloK8.DAL.Models;

namespace AlloK8.PL.Models;

public class TaskUpdateVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
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

    public bool IsPriority { get; set; }

    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }

    public List<UserProfile> Users { get; set; } = [];
    public List<UserProfile> AllUsers { get; set; } = [];

    public List<Label> Labels { get; set; } = [];
    public List<Label> AllLabels { get; set; } = [];

    public List<int> LabelIds { get; set; } = [];
}