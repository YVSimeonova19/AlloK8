using System;
using System.Collections.Generic;

namespace AlloK8.DAL.Models;

public class Board
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public UserProfile? Creator { get; set; }
    public Project? Project { get; set; }
    public ICollection<UserProfile>? Users { get; set; }
    public ICollection<Column>? Columns { get; set; }
}