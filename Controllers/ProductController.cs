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
        
        //get product list
        //get product list
       [HttpGet("operator")]
        [Authorize]
        [Role("operator")]
        public async Task<IActionResult> getlist()
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var user = _userRepository.getUser(authUserId);
            int? branchId = user.BranchId;

            if (!branchId.HasValue)
            {
                return NotFound("User not found");
            }

            var products = await _ProductRepository.ProductBalance(branchId.Value);

            if (products == null || !products.Any())
            {
                return NotFound();
            }

            return Ok(products);
        }
    




            //get product list for manager
       [HttpGet("manager")]
        [Authorize]
        [Role("manager")]
        public async Task<IActionResult> getlistForManager()
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var user = _userRepository.getUser(authUserId);
            int? branchId = user.BranchId;
            int? OrganizationId = user.OrganizationId;

            if (!OrganizationId.HasValue || !branchId.HasValue)
            {
                return NotFound("User not found or organization/branch not specified");
            }

            var products = await _ProductRepository.BalanceManager(OrganizationId.Value, null);

            if (products == null || !products.Any())
            {
                return NotFound();
            }

            return Ok(products);
        }


                
    }
}
