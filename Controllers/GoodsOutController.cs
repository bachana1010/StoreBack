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

    public class GoodsOutController : ControllerBase
    {
        private readonly IGoodsOutRepository _GoodsOutRepository;
        private readonly IUserRepository _userRepository;
                            
        public GoodsOutController(IGoodsOutRepository GoodsOutController, IUserRepository userRepository)
        {
            _GoodsOutRepository = GoodsOutController;
            _userRepository = userRepository;
        }


        [HttpPost("")]
        [Authorize]
        [Role("operator")]
        public async Task<IActionResult> AddGoodsOut([FromBody] MakeGoodsOutViewModel model)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var user = _userRepository.getUser(authUserId);
            Console.WriteLine(user);

            if (user == null)
            {
                return NotFound("User not found");
            }

            try
            {
                int goodsOutId = await _GoodsOutRepository.MakeGoodsOut(model, user);

                return Ok(new { message = "Goods out operation was successful.", goodsOutId = goodsOutId });
            }
            catch (Exception e)
            {
                // If there's an exception, return a BadRequest with the error message
                return BadRequest(new { error = e.Message });
            }
        }

           //getGoodOut
            [HttpGet]
            [Authorize]
            [Role("manager")]
            public async Task<IActionResult> GetGoodsOut([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5, [FromQuery] string quantityOperator = null, [FromQuery] float? quantityValue = null, [FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null)
            {
                var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(authUserIdString, out int authUserId))
                {
                    return BadRequest("Invalid user ID");
                }

                var user = _userRepository.getUser(authUserId);
                int? branchId = user.BranchId;
                int? organizationId = user.OrganizationId;

                var goodsOut = await _GoodsOutRepository.getGoodsOut(organizationId.Value, null, pageNumber, pageSize, quantityOperator, quantityValue, dateFrom, dateTo);

                if (goodsOut.TotalCount == null || !goodsOut.Results.Any())
                {
                    return NotFound();
                }

                return Ok(goodsOut);
            }










    }
}
