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
using System.Collections.Generic;
using BC = BCrypt.Net.BCrypt;
using StoreBack.ViewModels;


namespace StoreBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   public class AuthController : ControllerBase
{
      private readonly IAuthRepository _authRepository;
        private readonly StoreBack.ViewModels.JwtSettings _jwtSettings;
    
    public AuthController(IAuthRepository authRepository, IOptions<StoreBack.ViewModels.JwtSettings> jwtSettings)
    {
        _authRepository = authRepository;
        _jwtSettings = jwtSettings.Value;

    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterOrganizationViewModel model)
    {
        try
        {
            // Hash the password before sending it
            model.Password = BC.HashPassword(model.Password);

            var userId = await _authRepository.RegisterOrganizationAndUser(model);

            return Ok(new { message = "User registered successfully.", userId = userId });
        }
        catch(Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    } 



        //login




        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] StoreBack.ViewModels.LoginRequest model)
        {
        // try
        // {
        var user = await _authRepository.LoginUser(model.Email, model.Password);
        Console.WriteLine("63 xazi");
        Console.WriteLine(user);
        Console.WriteLine(model.Email);
        Console.WriteLine(model.Password);




        if (user == null)
        {
        Console.WriteLine("67 xazi");

        return Unauthorized();
        }

        Console.WriteLine(user.Username);
        Console.WriteLine(user.Id);
        Console.WriteLine(user.Email);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var claims = new[]
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim("email", user.Email),
                new Claim("organizationId", user.OrganizationId.ToString()),
                new Claim("role", user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { Token = tokenString, user = user });
        }
        // catch (Exception e)
        // {
        //     return BadRequest(new { error = e.Message });
        // }
        }



}
 



    