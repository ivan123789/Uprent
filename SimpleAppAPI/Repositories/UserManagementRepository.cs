using Microsoft.EntityFrameworkCore;
using SimpleAppAPI.Interfaces;
using SimpleAppAPI.Mappings;
using SimpleAppAPI.Models;
using SimpleAppEntityLibrary.DTOs;

namespace SimpleAppAPI.Repositories
{
    public class UserManagementRepository : IUsersRepository
    {
        private readonly MyDbContext _context;
        public UserManagementRepository(MyDbContext context)
        {
            _context = context;
        }
        public async Task<List<UserDto>> GetUsers()
        {
            List<UserDto> userDtos = new List<UserDto>();

            try
            {
                var users = await _context.Users.Include(u => u.UserRoles).ToListAsync();

                foreach (var user in users)
                {
                    userDtos.Add(UserManagementMapping.MapUserToUserDto(user));
                }
            }
            catch
            {
                throw;
            }

            return userDtos;
        }

        public async Task<List<RoleDto>> GetRoles()
        {
            List<RoleDto> roleDtos = new List<RoleDto>();

            try
            {
                var roles = await _context.Roles.ToListAsync();

                foreach (var role in roles)
                {
                    roleDtos.Add(UserManagementMapping.MapRoleToRoleDto(role));
                }
            }
            catch
            {
                throw;
            }

            return roleDtos;
        }

        public async Task SaveUserRoles(List<UserRoleSaveRequestDto> userRoleDtos)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Assume that all DTOs in the list refer to the same user, so get the UserId from the first item
                    var userId = await SaveUser(userRoleDtos[0]);

                    // Loop through each role DTO and handle each case (add or update/soft delete)
                    foreach (var userRoleDto in userRoleDtos)
                    {
                        var existingUserRole = await _context.UserRoles
                            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == userRoleDto.RoleId);

                        if (existingUserRole != null)
                        {
                            if (!userRoleDto.Visible)
                            {
                                // If the role is now deselected, perform a soft delete (set Visible to false)
                                existingUserRole.Visible = false;
                                existingUserRole.ModifiedByUserId = userRoleDto.ModifiedByUserId;
                                existingUserRole.ModifiedDate = DateTime.UtcNow;

                                _context.UserRoles.Update(existingUserRole);
                            }
                        }
                        else if (userRoleDto.Visible)
                        {
                            // If it's a new role (not in the database) and it is selected (Visible = true), add it
                            var newUserRole = new UserRole
                            {
                                UserId = userId,
                                RoleId = userRoleDto.RoleId,
                                CreatedDate = DateTime.UtcNow,
                                CreatedByUserId = userRoleDto.CreatedByUserId,
                                ModifiedByUserId = userRoleDto.ModifiedByUserId,
                                ModifiedDate = DateTime.UtcNow,
                                Visible = true,
                                Version = 1
                            };

                            _context.UserRoles.Add(newUserRole);
                        }
                    }

                    // Save changes for both user and user roles
                    await _context.SaveChangesAsync();

                    // Commit transaction
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    // Rollback transaction in case of error
                    await transaction.RollbackAsync();
                    throw new Exception("Error saving user roles", ex);
                }
            }
        }

        private async Task<int> SaveUser(UserRoleSaveRequestDto userDto)
        {
            try
            {
                // Check if the user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userDto.UserId);

                if (existingUser != null)
                {
                    // Update the existing user
                    existingUser.Username = userDto.Username;
                    existingUser.ModifiedByUserId = userDto.ModifiedByUserId;
                    existingUser.ModifiedDate = DateTime.UtcNow;

                    _context.Users.Update(existingUser);
                }
                else
                {
                    // Create a new user
                    var newUser = new User
                    {
                        Username = userDto.Username,
                        CreatedByUserId = userDto.CreatedByUserId,
                        ModifiedByUserId = userDto.CreatedByUserId,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        Visible = true,
                        Version = 1
                    };

                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                    // Return the ID of the newly created user
                    return newUser.UserId;
                }

                await _context.SaveChangesAsync();

                // Return the ID of the updated user
                return existingUser.UserId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving user", ex);
            }
        }

        public async Task DeleteUser(int userId, int modifiedByUserId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Retrieve the user to be deleted
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.UserId == userId);

                    if (user == null)
                    {
                        // Handle the case where the user is not found or already deleted
                        throw new InvalidOperationException("User not found or already deleted.");
                    }

                    // Soft delete userRoles associated with the user
                    var userRoles = await _context.UserRoles
                        .Where(ur => ur.UserId == userId)
                        .ToListAsync();

                    foreach (var userRole in userRoles)
                    {
                        userRole.Visible = false;
                        userRole.ModifiedByUserId = modifiedByUserId;
                        userRole.ModifiedDate = DateTime.UtcNow;

                        _context.UserRoles.Update(userRole);
                    }

                    // Perform the soft delete of the user
                    user.Visible = false;
                    user.ModifiedByUserId = modifiedByUserId;
                    user.ModifiedDate = DateTime.UtcNow;

                    // Update the user entity
                    _context.Users.Update(user);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    throw new Exception("Error deleting user and associated roles", ex);
                }
            }
        }


    }
}
