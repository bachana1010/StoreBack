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

        [HttpPost("")]
        [Role("administrator")]
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

                return Ok(new { message = "Branch created successfully." });
            }
            catch(Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }


        //delete user
        [HttpDelete("{BranchId}")]
        [Authorize]
        [Role("administrator")]
        public async Task<IActionResult> DeleteBranch(int BranchId)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            User authUser = _userRepository.getUser(authUserId);
            Branches branch = _branchRepository.GetBranch(BranchId);

            // iuzeris hsemowmeba tua avgorizebuli
            if (authUser == null)
            {
                return NotFound("Authorized user not found");
            }

            // brenchis hsemowmeba
            if (branch == null)
            {
                return NotFound("Branch not found");
            }
            
            if (authUser.OrganizationId != branch.OrganizationId)
            {
                return Unauthorized("This user does not belong to the branch's organization");
            }

            // delete  branch
            await _branchRepository.DeleteBranch(BranchId);

                return Ok(new { message = "Branch Deleted successfully." });
        }



        //get branches list
        [HttpGet("")]
        [Authorize]
        [Role("administrator")]
        public async Task<IActionResult> GetBranches([FromQuery] string? BrancheName, [FromQuery] string? Username, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            //useris amogeba aidit
            User authUser = _userRepository.getUser(authUserId);

            var pagedBranches = await _branchRepository.GetBranches(BrancheName, Username, authUser.OrganizationId, pageNumber, pageSize);

            return Ok(new 
            { 
                Branches = pagedBranches.Results, 
                TotalCount = pagedBranches.TotalCount 
            });
        }

        [HttpGet("{Id}")]
        [Authorize]
        [Role("administrator")]
        public async Task<IActionResult> GetBranch(int Id)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var branch = _branchRepository.GetBranch(Id);

            return Ok(branch);
        }



        [HttpPut("{id}")]
        [Authorize]
        [Role("administrator")]
        public async Task<IActionResult> UpdateBranch(int id,[FromBody] UpdateBranchViewModel model)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            //branchis amogeba GetBranch - it
            var branch = _branchRepository.GetBranch(id);

            try
            {
                await _branchRepository.UpdateBranch(id,model);

                if(true)
                {
                    return Ok(new { message = "Branch updated successfully." });
                }
                else
                {
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
