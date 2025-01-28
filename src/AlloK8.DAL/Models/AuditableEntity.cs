using System;

namespace AlloK8.DAL.Models;

public class AuditableEntity : Entity
{
    public DateTime? CreatedOn { get; set; }

    public int CreatedByUserId { get; set; }
    public UserProfile CreatedByUser { get; set; } = null!;

    public DateTime? UpdatedOn { get; set; }

    public int UpdatedByUserId { get; set; }
    public UserProfile UpdatedByUser { get; set; } = null!;
}