using Microsoft.AspNetCore.Mvc;
using StoreBack.Models;
using StoreBack.Repositories;
using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using StoreBack.ViewModels;
using BC = BCrypt.Net.BCrypt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using StoreBack.Authorizations;

namespace StoreBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly StoreBack.ViewModels.JwtSettings _jwtSettings;    
                            
        public UserController(IUserRepository userRepository, IOptions<StoreBack.ViewModels.JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("")]
        [Authorize]
        [Role("Administrator")]
        public async Task<IActionResult> CreateUser([FromBody] AddUserViewModel model)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var user = _userRepository.getUser(authUserId);

            try
            {
                int userId = await _userRepository.AddUser(model, user);

                return Ok(new { message = "User created successfully.", userId = userId });
            }
            catch(Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        [Role("Administrator")]
        public async Task<IActionResult> UpdateUser(int id,[FromBody] UpdateserViewModel model)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            // You may want to add a condition to check if the user has permission to update the resource
            // For example, if the user can only update their own profile:
            if(authUserId == id)
            {
                return Unauthorized("You do not have permission to update this resource");
            }

            // Hash password only if it exists
            if (!string.IsNullOrEmpty(model.Password))
            {
                model.Password = BC.HashPassword(model.Password);
            }

            try
            {
                await _userRepository.UpdateUser(id,model);

                return Ok(new { message = "User updated successfully." });
            }
            catch(Exception e)
            {
                return BadRequest(new { error = e.Message });
            }

        }


        //delete user
        [HttpDelete("{id}")]
        [Authorize]
        [Role("Administrator")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            User authUser = _userRepository.getUser(authUserId);
            User deleteUser = _userRepository.getUser(id);

            if (authUser.OrganizationId != deleteUser.OrganizationId) {
                return Unauthorized("This user is not in your Organization");
            }

        
            // check if the authorized user is the same as the user being deleted
            if (authUserId == id)
            {
                return Unauthorized();
            }

            await _userRepository.DeleteUser(id);

            return Ok();
        }


        //getusers
        [HttpGet("")]
        [Authorize]
        [Role("Administrator")]
        public async Task<IActionResult> GeteUsers()
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            User authUser = _userRepository.getUser(authUserId);

            var users = await _userRepository.GetUsers(authUser.OrganizationId);

            return Ok(users);
        }


        //getBranch

        [HttpGet("{Id}")]
        [Authorize]
        [Role("Administrator")]
        public async Task<IActionResult> GetUser(int Id)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            // User authUser = await _userRepository.getUser(authUserId);

             var user = _userRepository.getUser(Id);

            return Ok(user);
        }


    }
}
