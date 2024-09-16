using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAppEntityLibrary.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }

        public string Username { get; set; } = null!;

        public int CreatedByUserId { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual List<int>? UserRoleIds { get; set; }

    }
}
