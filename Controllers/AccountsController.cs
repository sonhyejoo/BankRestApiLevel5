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
        private readonly AccountService _service;

        public AccountsController(AccountContext context, AccountService accountService)
        {
            _context = context;
            _service = accountService;
        }

        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }


        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Account>> CreateAccount(CreateAccountRequest request)
        {
            var createdResult = await _service.Create(request);
            var success = createdResult.IsSuccess;
            var createdAccount =  createdResult.Result;
            if (!success)
            {
                return BadRequest(new {Error = createdResult.ErrorMessage});
            }

            return CreatedAtAction(nameof(GetAccount), new { id = createdAccount.Id }, createdAccount);
        }
        
        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.InternalId == id);
        }
    }
}
