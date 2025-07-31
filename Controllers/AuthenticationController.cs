using BankRestApi.Interfaces;
using BankRestApi.Models.DTOs;
using BankRestApi.Models.DTOs.Requests;
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
        /// <param name="request">User name and password</param>
        /// <returns>Access token and refresh token to access routes requiring authentication</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Token))]
        [ProducesResponseType(StatusCodes.Status400BadRequest,  Type=typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<Token>> Login(LoginRequest request)
        {
            var result = await _service.CreateAccessTokenAsync(request);

            return result.IsSuccess ? Ok(result.Result) : StatusCode((int)result.StatusCode!, result.ErrorMessage);
        }

        /// <summary>
        /// Use valid refresh token to get new access token
        /// </summary>
        /// <param name="request">User name and refresh token to use for renewal</param>
        /// <returns>New access token and refresh token</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Token))]
        [ProducesResponseType(StatusCodes.Status400BadRequest,  Type=typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<ActionResult<Token>> Refresh(RefreshTokenRequest request)
        {
            var result = await _service.RefreshTokenAsync(request);

            return result.IsSuccess ? Ok(result.Result) : StatusCode((int)result.StatusCode!, result.ErrorMessage);
        }

        /// <summary>
        /// Revoke refresh token to prevent access until next valid login
        /// </summary>
        /// <param name="request">User name and refresh token to revoke</param>
        /// <returns>No content</returns>
        [HttpPost("revoke")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest,  Type=typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> Revoke(RevokeRequest request)
        {
            var result = await _service.RevokeRefreshToken(request);

            return StatusCode((int)result.StatusCode!, result.ErrorMessage);
        }
    }
}
