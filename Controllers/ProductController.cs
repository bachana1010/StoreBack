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
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _ProductRepository;
        private readonly IUserRepository _userRepository;
        
        public ProductController(IProductRepository productRepository, IUserRepository userRepository)
        {
            _ProductRepository = productRepository;
            _userRepository = userRepository;
        }
        
        //product list
        [HttpGet]
        [Authorize]
        [Role("operator", "manager")]
        public async Task<IActionResult> getlist(
            [FromQuery] string name = null,
            [FromQuery] string priceOperator = null,
            [FromQuery] decimal? priceValue = null,
            [FromQuery] string quantityOperator = null,
            [FromQuery] decimal? quantityValue = null,
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 5)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var user = _userRepository.getUser(authUserId);

            PagedResult<GetBarcodeBalanceViewModel> products = new PagedResult<GetBarcodeBalanceViewModel>();

            //rolebis shemowmeba romeli rolia
            if (user.Role == "operator") {
                products = await _ProductRepository.ProductBalance(user.BranchId, null, name, priceOperator, priceValue, quantityOperator, quantityValue, pageNumber, pageSize);
            } else if (user.Role == "manager") {
                products = await _ProductRepository.ProductBalance(null, user.OrganizationId, name, priceOperator, priceValue, quantityOperator, quantityValue, pageNumber, pageSize);
            }

            if (products.TotalCount == 0)
            {
                return NotFound();
            }

            return Ok(products);
        }

        //dashbordistvis listebis gadacema, cardebistvis
        [HttpGet("dashboard")]
        [Authorize]
        [Role("administrator")]        
        
        public async Task<IActionResult> GetDashboardData()
        {

            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var user = _userRepository.getUser(authUserId);

            if (user.OrganizationId == null)
            {
                return BadRequest("OrganizationId for user is null");
            }

            var dashboardData = await _ProductRepository.GetDashboardData((int)user.OrganizationId);

            return Ok(dashboardData);
        }



}}
