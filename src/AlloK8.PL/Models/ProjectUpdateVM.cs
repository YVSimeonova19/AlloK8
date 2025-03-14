using System.Collections.Generic;
using AlloK8.DAL.Models;

namespace AlloK8.PL.Models;

public class ProjectUpdateVM
{
    public int Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }

    public List<UserProfile> Users { get; set; } = [];

    public List<UserProfile> AllUsers { get; set; } = [];
}