using System;

namespace AlloK8.DAL.Models;

public class Task
{
    public int TaskId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public string? Priority { get; set; }

    public DateTime DueDate { get; set; }

    public int CreatedBy { get; set; }

    public int AssignedTo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public User? Creator { get; set; }

    public User? Assignee { get; set; }
}