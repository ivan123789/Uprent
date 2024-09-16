using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAppEntityLibrary.DTOs
{
    public class UserRoleSaveRequestDto
    {
        public int RoleId { get; set; }

        public int UserId { get; set; }

        public string? Username { get; set; }

        public int CreatedByUserId { get; set; }

        public int? ModifiedByUserId { get; set; }

        public bool Visible { get; set; }
    }
}
