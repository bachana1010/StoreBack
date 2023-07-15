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

    public class GoodSinController : ControllerBase
    {
        private readonly IGoodsinRepository _GoodsinRepository;
        private readonly IUserRepository _userRepository;
                            
        public GoodSinController(IGoodsinRepository GoodSinController, IUserRepository userRepository)
        {
            _GoodsinRepository = GoodSinController;
            _userRepository = userRepository;
        }

        //goodsinis damateba

        [HttpPost("")]
        [Authorize]
        [Role("operator")]
        
        public async Task<IActionResult> AddGoodSin([FromBody] MakeGoodsInViewModel model)
        {
            //igive procesi
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }


            //useris amogeba
            var user = _userRepository.getUser(authUserId);

              if (user == null)
                {
                    return NotFound("User not found");
                }

            try
            {
                int userId = await _GoodsinRepository.MakeGoodsIn(model, user);

                return Ok(new { message = "Goddsin Added successfully.", userId = userId });
            }
            catch(Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }



        //getbarcode
        [HttpGet("{barcodetext}")]
        [Authorize]
        [Role("operator")]
        public async Task<IActionResult> GetBarcode(string barcodetext)
        {
            if (string.IsNullOrEmpty(barcodetext))
            {
                return BadRequest("Invalid barcode text");
            }

            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var user = _userRepository.getUser(authUserId);
            if(user == null || user.BranchId == null)
            {
                return BadRequest("User not found or has no assigned branch");
            }

            var barcode = _GoodsinRepository.getBarcode(barcodetext, user.BranchId.Value); 

            if (barcode == null)
            {
                return NotFound();
            }

            return Ok(barcode);
        }



       //getGoodsin
        [HttpGet]
        [Authorize]
        [Role("manager")]
        public async Task<IActionResult> GetGoodsin([FromQuery] string quantityOperator = null, [FromQuery] decimal? quantity = null, [FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null, [FromQuery] int? branchId = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
               
                // paginationis damateba
                if(pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest("Invalid pageNumber or pageSize");
                }

                //aqac igive procesi
                var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(authUserIdString, out int authUserId))
                {
                    return BadRequest("Invalid user ID");
                }

                //useris amogeba da organzzation id-is
                var user = _userRepository.getUser(authUserId);
                int? userOrganizationId = user.OrganizationId;

                //repositorishi gadasrola
                var goodsIn = await _GoodsinRepository.GetGoodsIn(userOrganizationId.Value, branchId, quantityOperator, quantity, dateFrom, dateTo, pageNumber, pageSize);

                if (goodsIn.TotalCount == null || !goodsIn.Results.Any())
                {
                    return NotFound();
                }

      

                return Ok(goodsIn);
            }
            catch(Exception e)
            {

                return StatusCode(500, e.Message);
            }
        }
    }
}
