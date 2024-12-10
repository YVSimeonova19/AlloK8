using System.Collections;
using System.Collections.Generic;
using AlloK8.DAL.Models;

namespace AlloK8.DAL.Models;

public class UserProfile
{
    public ICollection<Task>? Tasks { get; set; }

    public ICollection<Notification>? Notifications { get; set; }

    public ICollection<Collaboration>? Collaborations { get; set; }

    public ICollection<Message>? Messages { get; set; }
}