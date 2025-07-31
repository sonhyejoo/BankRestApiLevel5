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
        private readonly IAuthenticationService _service;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _service = authenticationService;
        }

        /// <summary>
        /// Login using name and password for bank API authentication
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Access token and refresh token to access routes requiring authentication</returns>
        [HttpPost("login")]
        public async Task<ActionResult<Token>> Login(LoginRequest request)
        {
            var result = await _service.CreateAccessTokenAsync(request);

            return result.IsSuccess ? Ok(result.Result) : StatusCode((int)result.StatusCode!, result.ErrorMessage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>New access token and refresh token</returns>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<Token>> Refresh(RefreshTokenRequest request)
        {
            var result = await _service.RefreshTokenAsync(request);

            return result.IsSuccess ? Ok(result.Result) : StatusCode((int)result.StatusCode!, result.ErrorMessage);
        }

        /// <summary>
        /// Revoke refresh token to prevent access until next valid login
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns>No content</returns>
        [HttpPost("revoke")]
        public async Task<ActionResult<Token>> Revoke(RevokeRequest request)
        {
            var result = await _service.RevokeRefreshToken(request);

            return StatusCode((int)result.StatusCode!, result.ErrorMessage);
        }
    }
}
