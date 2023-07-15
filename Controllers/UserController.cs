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
        

        //create user
       [HttpPost("")]
        [Authorize]
        [Role("administrator")]
        public async Task<IActionResult> CreateUser([FromBody] AddUserViewModel model)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var user = _userRepository.getUser(authUserId);

            // Check if a user with the same email already exists
            var existingUser = _userRepository.getUserByEmail(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { error = "User already exists." });
            }

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


        //user-is update

        [HttpPut("{id}")]
        [Authorize]
        [Role("administrator")]
        public async Task<IActionResult> UpdateUser(int id,[FromBody] UpdateserViewModel model)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
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
        [Role("administrator")]
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

        
            if (authUserId == id)
            {
                return Unauthorized();
            }

            await _userRepository.DeleteUser(id);

                return Ok(new { message = "User Deleted successfully." });
        }


   //getusers
        [HttpGet("")]
        [Authorize]
        [Role("administrator")]
        public async Task<IActionResult> GetUsers([FromQuery] UserFilterViewModel filter, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            User authUser = _userRepository.getUser(authUserId);

            var pagedUsers = await _userRepository.GetUsers(filter, authUser.OrganizationId, pageNumber, pageSize);

            return Ok(new 
            { 
                Users = pagedUsers.Results, 
                TotalCount = pagedUsers.TotalCount 
            });
        }



        //getBranch

        [HttpGet("{Id}")]
        [Authorize]
        [Role("administrator")]
        public async Task<IActionResult> GetUser(int Id)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }


             var user = _userRepository.getUser(Id);

            return Ok(user);
        }


    }
}
