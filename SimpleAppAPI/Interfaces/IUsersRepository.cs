using SimpleAppAPI.Models;
using SimpleAppEntityLibrary.DTOs;

namespace SimpleAppAPI.Interfaces
{
    public interface IUsersRepository
    {
        Task<List<UserDto>> GetUsers();

        Task<List<RoleDto>> GetRoles();

        Task SaveUserRoles(List<UserRoleSaveRequestDto> userRoleDto);

        Task DeleteUser(int userId, int modifiedByUserId);

    }
}
