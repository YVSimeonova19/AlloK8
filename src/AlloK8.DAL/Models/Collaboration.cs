using System;

namespace AlloK8.DAL.Models;

public class Collaboration
{
    public int CollaborationId { get; set; }

    public int TaskId { get; set; }

    public int UserId { get; set; }

    public string? Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public Task? Task { get; set; }

    public UserProfile? User { get; set; }
}