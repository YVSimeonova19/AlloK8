using System;
using System.Collections;
using System.Collections.Generic;
using AlloK8.DAL.Models;

namespace AlloK8.DAL.Models;

public class UserProfile : Entity
{
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }

    public List<Task> Tasks { get; } = [];

    public Task? TaskCreated { get; set; }
    public Task? TaskUpdated { get; set; }

    public List<Board> Boards { get; } = [];

    public Board? BoardCreated { get; set; }
    public Board? BoardUpdated { get; set; }

    public List<Project> Projects { get; } = [];

    public List<Project> ProjectsCreated { get; } = [];

    public List<Project> ProjectsUpdated { get; } = [];
}