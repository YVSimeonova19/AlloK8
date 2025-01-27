using System;
using System.Collections;
using System.Collections.Generic;

namespace AlloK8.DAL.Models;

public class Project
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public UserProfile? Creator { get; set; }
    public ICollection<UserProfile>? Users { get; set; }
    public ICollection<Board>? Boards { get; set; }
}