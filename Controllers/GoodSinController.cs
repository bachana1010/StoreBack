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
        public async Task<IActionResult> GetGoodsin([FromQuery] string quantityOperator = null, [FromQuery] decimal? quantity = null, [FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null, [FromQuery] int? branchId = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                // Log the start of the call
                Console.WriteLine("GetGoodsin started");
                Console.WriteLine(quantity);
                        Console.WriteLine(quantityOperator);
                                        Console.WriteLine(pageNumber);




                // Add validation for pageNumber and pageSize
                if(pageNumber < 1 || pageSize < 1)
                {
                    return BadRequest("Invalid pageNumber or pageSize");
                }

                var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(authUserIdString, out int authUserId))
                {
                    return BadRequest("Invalid user ID");
                }

                var user = _userRepository.getUser(authUserId);
                int? userOrganizationId = user.OrganizationId;

                var goodsIn = await _GoodsinRepository.GetGoodsIn(userOrganizationId.Value, branchId, quantityOperator, quantity, dateFrom, dateTo, pageNumber, pageSize);

                if (goodsIn.TotalCount == null || !goodsIn.Results.Any())
                {
                    return NotFound();
                }

                // Log the end of the call
                Console.WriteLine("GetGoodsin finished");

                return Ok(goodsIn);
            }
            catch(Exception e)
            {
                // Log the exception
                Console.WriteLine($"GetGoodsin failed with exception: {e.Message}");

                // Return server error status code with exception message
                return StatusCode(500, e.Message);
            }
        }








    }
}
