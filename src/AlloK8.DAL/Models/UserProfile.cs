using System.Collections;
using System.Collections.Generic;
using AlloK8.DAL.Models;

namespace AlloK8.DAL.Models;

public class UserProfile : ApplicationUser
{
    public List<Task> Tasks { get; } = [];
    public Task? TaskCreated { get; set; }
    public Task? TaskUpdated { get; set; }

    public List<Board> Boards { get; } = [];

    public List<Project> Projects { get; } = [];
}