using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BankRestApi.Models;
using BankRestApi.Models.DTOs.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BankRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AccountContext _context;
        private readonly IConfiguration _config;

        public AuthenticationController(AccountContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        
        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Authenticate(AuthenticationRequest request)
        {
            var user = await ValidateUserCredentials(request.AccountName, request.Password);
            if (user is null)
            {
                return Unauthorized();
            }

            var securityKey = new SymmetricSecurityKey(
                Convert.FromBase64String(_config["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>()
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("name", user.AccountName)
            };

            var jwtSecurityToken = new JwtSecurityToken(
                _config["Authentication:Issuer"],
                _config["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }
        
        private async Task<User?> ValidateUserCredentials(string accountName, string password)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.AccountName == accountName);
            return currentUser is null || currentUser.HashedPassword != password ? null : currentUser;
        } 
    }
}
