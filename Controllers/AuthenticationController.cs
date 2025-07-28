using BankRestApi.Interfaces;
using BankRestApi.Models;
using BankRestApi.Models.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BankRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IUserService _service;

        public AuthenticationController(AppDbContext context, IConfiguration config, IUserService userService)
        {
            _context = context;
            _config = config;
            _service = userService;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Authenticate(AuthenticationRequest request)
        {
            var result = await _service.ValidateUserCredentials(request);

            return result.IsSuccess ? Ok(result.Result) : StatusCode((int)result.StatusCode!, result.ErrorMessage);
        }
    }
}
