using System;
using System.Collections;
using System.Collections.Generic;

namespace AlloK8.DAL.Models;

public class Project : AuditableEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public ICollection<Task> Tasks { get; } = new List<Task>();

    public List<UserProfile> Users { get; set;  } = [];
    public List<Label> Labels { get; } = [];
}