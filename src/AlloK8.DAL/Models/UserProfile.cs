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

    public List<Task> TasksCreated { get; set; } = [];
    public List<Task> TasksUpdated { get; set; } = [];

    public List<Project> Projects { get; set; } = [];

    public List<Project> ProjectsCreated { get; } = [];

    public List<Project> ProjectsUpdated { get; } = [];
}