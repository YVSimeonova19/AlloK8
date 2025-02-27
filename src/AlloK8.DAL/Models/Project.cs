using System;
using System.Collections;
using System.Collections.Generic;

namespace AlloK8.DAL.Models;

public class Project : AuditableEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public ICollection<Column> Columns { get; } = new List<Column>();

    public List<UserProfile> Users { get; set;  } = [];
}