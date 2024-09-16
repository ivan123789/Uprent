using System;
using System.Collections.Generic;

namespace SimpleAppAPI.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public int CreatedByUserId { get; set; }

    public int? ModifiedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool Locked { get; set; }

    public bool Visible { get; set; }

    public int Version { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
