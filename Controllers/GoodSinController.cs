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

        [HttpPost("")]
        [Authorize]
        [Role("operator")]
        
        public async Task<IActionResult> AddGoodSin([FromBody] MakeGoodsInViewModel model)
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
                int userId = await _GoodsinRepository.MakeGoodsIn(model, user);

                return Ok(new { message = "Goddsin Added successfully.", userId = userId });
            }
            catch(Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        //getbarcodes

        [HttpGet("{barcodetext}")]
        [Authorize]
        [Role("operator")]
        public async Task<IActionResult> GetBarcode(string barcodetext)
        {
            if (string.IsNullOrEmpty(barcodetext))
            {
                return BadRequest("Invalid barcode text");
            }

            var barcode = _GoodsinRepository.getBarcode(barcodetext);

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
        public async Task<IActionResult> GetGoodsin()
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
              if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var user = _userRepository.getUser(authUserId);
            int? branchId = user.BranchId;
            int? OrganizationId = user.OrganizationId;



            var goodsIn = await _GoodsinRepository.getGoodsin(OrganizationId.Value, null);

            if (goodsIn == null || !goodsIn.Any())
            {
                return NotFound();
            }

            return Ok(goodsIn);
        }








    }
}
