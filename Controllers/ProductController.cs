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
        
        [HttpGet]
        [Authorize]
        [Role("operator", "manager")]
        public async Task<IActionResult> getlist([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(authUserIdString, out int authUserId))
            {
                return BadRequest("Invalid user ID");
            }

            var user = _userRepository.getUser(authUserId);

            PagedResult<GetBarcodeBalanceViewModel> products = new PagedResult<GetBarcodeBalanceViewModel>();

            if (user.Role == "operator") {
                products = await _ProductRepository.ProductBalance(user.BranchId, null, pageNumber, pageSize);
            } else if (user.Role == "manager") {
                products = await _ProductRepository.ProductBalance(null, user.OrganizationId, pageNumber, pageSize);
            }

            if (products.TotalCount == 0)
            {
                return NotFound();
            }

            return Ok(products);
        }


}}

    //    [HttpGet("manager")]
    //     [Authorize]
    //     [Role("manager")]
    //     public async Task<IActionResult> getlistForManager([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)

    //     {
    //         var authUserIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

    //         if (!int.TryParse(authUserIdString, out int authUserId))
    //         {
    //             return BadRequest("Invalid user ID");
    //         }

    //         var user = _userRepository.getUser(authUserId);
    //         int? branchId = user.BranchId;
    //         int? OrganizationId = user.OrganizationId;

    //         if (!OrganizationId.HasValue || !branchId.HasValue)
    //         {
    //             return NotFound("User not found or organization/branch not specified");
    //         }

    //         var products = await _ProductRepository.BalanceManager(OrganizationId.Value, null, pageNumber, pageSize);


    //         if (products.TotalCount == null || !products.Results.Any())
    //         {
    //             return NotFound();
    //         }

    //         return Ok(products);
    //     }    

