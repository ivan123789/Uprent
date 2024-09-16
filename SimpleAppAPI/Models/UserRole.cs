using System;
using System.Collections.Generic;

namespace SimpleAppAPI.Models;

public partial class UserRole
{
    public int UserRoleId { get; set; }

    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string? Comment { get; set; }

    public int CreatedByUserId { get; set; }

    public int? ModifiedByUserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool Locked { get; set; }

    public bool Visible { get; set; }

    public int Version { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
