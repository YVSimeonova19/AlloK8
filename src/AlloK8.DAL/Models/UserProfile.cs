using System.Collections;
using System.Collections.Generic;
using AlloK8.DAL.Models;

namespace AlloK8.DAL.Models;

public class UserProfile : ApplicationUser
{
    public ICollection<Task>? Tasks { get; set; }

    public ICollection<Notification>? Notifications { get; set; }
}