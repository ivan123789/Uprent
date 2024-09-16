using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleAppAPI.Interfaces;
using SimpleAppAPI.Models;
using SimpleAppEntityLibrary.DTOs;

namespace SimpleAppAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUsersRepository _usersRepository;
        public UserManagementController( IUsersRepository usersRepository ) 
        {
            _usersRepository = usersRepository;
        }

        [HttpGet("/users", Name = "GetUsers")]
        public async Task<ActionResult<UserDto>> GetUsers()
        {
            try
            {
                var users = await _usersRepository.GetUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("/roles", Name = "GetRoles")]
        public async Task<ActionResult<RoleDto>> GetRoles()
        {
            try
            {
                var roles = await _usersRepository.GetRoles();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("/userRoles", Name = "SaveUserRoles")]
        public async Task<ActionResult> SaveUserRoles([FromBody] List <UserRoleSaveRequestDto> userRoleDtos)
        {
            if (userRoleDtos == null || !userRoleDtos.Any())
            {
                return BadRequest("UserRoleDtos cannot be null or empty.");
            }

            try
            {
                await _usersRepository.SaveUserRoles(userRoleDtos);

                return CreatedAtAction(nameof(SaveUserRoles), new { userRoleDtos[0].UserId }, userRoleDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("/soft-delete/{userId}", Name = "DeleteUser")]
        public async Task<ActionResult<RoleDto>> DeleteUser(int userId, [FromQuery] int modifiedByUserId)
        {
            try
            {
                await _usersRepository.DeleteUser(userId, modifiedByUserId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
