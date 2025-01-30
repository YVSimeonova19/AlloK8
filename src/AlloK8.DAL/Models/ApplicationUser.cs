using System;
using AlloK8.DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace AlloK8.DAL;

public class ApplicationUser : IdentityUser<Guid>
{
    public UserProfile? UserProfile { get; set; }
}
