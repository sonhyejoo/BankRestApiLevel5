using BankRestApi.Models.DTOs.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        // [HttpPost("authenticate")]
        // public ActionResult<string> Authenticate(AuthenticationRequest request)
        // {
        //     var user = ValidateUserCredentials(request.AccountName, request.Password);
        // }
        //
        // private object ValidateUserCredentials(string requestAccountName, string requestPassword)
        // {
        //     throw new NotImplementedException();
        // } 
    }
}
