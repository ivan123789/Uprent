using SimpleAppAPI.Models;
using SimpleAppEntityLibrary.DTOs;

namespace SimpleAppAPI.Mappings
{
    public static class UserManagementMapping
    {
        public static UserDto MapUserToUserDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                CreatedDate = user.CreatedDate,
                CreatedByUserId = user.CreatedByUserId,
                ModifiedDate = user.ModifiedDate,
                ModifiedByUserId = user.ModifiedByUserId,
                UserRoleIds = user.UserRoles
                    .Where(userRole => userRole.UserId == user.UserId)
                    .Select(userRole => userRole.RoleId) 
                    .ToList()
            };
        }

        public static RoleDto MapRoleToRoleDto(Role role)
        {
            return new RoleDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
            };
        }

    }
}
