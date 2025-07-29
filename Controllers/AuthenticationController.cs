using BankRestApi.Interfaces;
using BankRestApi.Models;
using BankRestApi.Models.DTOs;
using BankRestApi.Models.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IAuthenticationService _service;

        public AuthenticationController(
            AppDbContext context,
            IConfiguration config,
            IAuthenticationService authenticationService)
        {
            _context = context;
            _config = config;
            _service = authenticationService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<Token>> Login(AuthenticationRequest request)
        {
            var result = await _service.CreateAccessTokenAsync(request.AccountName, request.Password);

            return result.IsSuccess ? Ok(result.Result) : StatusCode((int)result.StatusCode!, result.ErrorMessage);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<Token>> Refresh(RefreshTokenRequest request)
        {
            var result = await _service.RefreshTokenAsync(request.AccountName, request.RefreshToken);

            return result.IsSuccess ? Ok(result.Result) : StatusCode((int)result.StatusCode!, result.ErrorMessage);
        }

        [HttpPost("revoke")]
        public async Task<ActionResult<Token>> Revoke(string refreshToken)
        {
            var result = await _service.RevokeRefreshToken(refreshToken);

            return StatusCode((int)result.StatusCode!, result.ErrorMessage);
        }
    }
}
