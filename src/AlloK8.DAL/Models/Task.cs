using System;

namespace AlloK8.DAL.Models;

public class Task
{
    public int TaskId { get; set; }

    public int UserId { get; set; }

    public string? Title { get; set; }
    public string? Description { get; set; }

    public int ColumnId { get; set; }
    public int Position { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }

    public int CreatedBy { get; set; }
    public int AssignedTo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public UserProfile? Creator { get; set; }
    public UserProfile? Assignee { get; set; }
    public Column? Column { get; set; }
}