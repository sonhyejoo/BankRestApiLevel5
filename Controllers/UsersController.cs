using BankRestApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BankRestApi.Models;
using BankRestApi.Models.DTOs.Requests;

namespace BankRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService userService)
        {
            _service = userService;
        }
        
        /// <summary>
        /// Create user account for authentication to use bank
        /// </summary>
        /// <param name="request">User name and password</param>
        /// <returns>Created account details</returns>
        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> Create(CreateUserRequest request)
        {
            var result = await _service.CreateUserAsync(request);

            return result.IsSuccess ? Ok(result.Result) : StatusCode((int)result.StatusCode!, result.ErrorMessage);
        }
    }
}
