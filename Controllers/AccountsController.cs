using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankRestApi.Models;
using BankRestApi.Models.DTOs;
using BankRestApi.Services;
using Account = BankRestApi.Models.Account;

namespace BankRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AccountContext _context;
        private readonly IAccountService _service;

        public AccountsController(AccountContext context, IAccountService accountService)
        {
            _context = context;
            _service = accountService;
        }

        // GET: api/Accounts/0b4b7e2b-ffd1-4acf-81b3-e51d48155217
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(Guid id)
        {
            var request = new GetAccountRequest(id);
            var getResult = await _service.Get(request);

            if (!getResult.IsSuccess)
            {
                return NotFound(new { Error = getResult.ErrorMessage });
            }

            return Ok(getResult.Result);
        }

        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Account>> CreateAccount(CreateAccountRequest request)
        {
            var createdResult = await _service.Create(request);
            var success = createdResult.IsSuccess;
            var createdAccount = createdResult.Result;
            if (!success)
            {
                return BadRequest(new { Error = createdResult.ErrorMessage });
            }

            return CreatedAtAction(nameof(GetAccount), new { id = createdAccount.Id }, createdAccount);
        }

        // POST: api/Accounts/0b4b7e2b-ffd1-4acf-81b3-e51d48155217/deposiits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id}/deposits")]
        public async Task<ActionResult<Account>> DepositAccount(Guid id, [FromBody] decimal amount)
        {
            var depositRequest = new TransactionRequest(Amount: amount, Id: id);
            var depositResult = await _service.Deposit(depositRequest);
            if (!depositResult.IsSuccess)
            {
                return BadRequest(new { Error = depositResult.ErrorMessage });
            }

            return Ok(new { updatedBalance = depositResult.Result });
        }
    }
}
