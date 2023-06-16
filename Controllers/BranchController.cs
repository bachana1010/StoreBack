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
    public class BranchController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly IBranchRepository _branchRepository;
    
        public BranchController(IBranchRepository BranchRepository, IUserRepository userRepository)
        {
            _branchRepository= BranchRepository;
            _userRepository = userRepository;
        }


        //add branch

        [HttpPost("")]
        [Role("Administrator")]
        [Authorize]
        public async Task<IActionResult> CreateBranche([FromBody] AddBranchViewModel model)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var user = _userRepository.getUser(authUserId);

            try
            {
                await _branchRepository.addBranch(model, user );

                return Ok(new { message = "User created successfully." });
            }
            catch(Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }


        //delete user
        [HttpDelete("{BranchId}")]
        [Authorize]
        [Role("Administrator")]
        public async Task<IActionResult> DeleteBranch(int BranchId)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            User authUser = _userRepository.getUser(authUserId);
            Branches branch = _branchRepository.GetBranch(BranchId);

            // Add null check for user
            if (authUser == null)
            {
                return NotFound("Authorized user not found");
            }

            // Add null check for branch
            if (branch == null)
            {
                return NotFound("Branch not found");
            }

            Console.WriteLine($"Auth User Organization ID: {authUser.OrganizationId}");
            Console.WriteLine($"Branch Organization ID: {branch.OrganizationId}");

            // check if the authorized user has the same organizationId as the branch
            if (authUser.OrganizationId != branch.OrganizationId)
            {
                return Unauthorized("This user does not belong to the branch's organization");
            }

            // delete the branch
            await _branchRepository.DeleteBranch(BranchId);

            return Ok();
        }


        //get branches list

        [HttpGet("")]
        [Authorize]
        [Role("Administrator")]
        public async Task<IActionResult> GetBranches()
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            User authUser = _userRepository.getUser(authUserId);

            var branches = await _branchRepository.GetBranches(authUser.OrganizationId);

            return Ok(branches);
        }



        //get brancheby id


        [HttpGet("{Id}")]
        [Authorize]
        [Role("Administrator")]
        public async Task<IActionResult> GetBranch(int Id)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            // User authUser = await _userRepository.getUser(authUserId);

             var branch = _branchRepository.GetBranch(Id);

            return Ok(branch);
        }



        [HttpPut("{id}")]
        [Authorize]
        [Role("Administrator")]
        public async Task<IActionResult> UpdateBranch(int id,[FromBody] UpdateBranchViewModel model)
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


            try
            {
                await _branchRepository.UpdateBranch(id,model);

                if(true)
                {
                    return Ok(new { message = "User updated successfully." });
                }
                else
                {
                    // If the update method returns false (i.e., no rows were updated), return a not found status
                    return NotFound("The user was not found or no changes were made.");
                }
            }
            catch(Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }




    }
}
