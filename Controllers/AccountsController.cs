using Microsoft.AspNetCore.Mvc;
using BankRestApi.Models.DTOs;
using BankRestApi.Services;
using Account = BankRestApi.Models.Account;

namespace BankRestApi.Controllers
{
    /// <summary>
    /// Controller for managing bank interactions.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountsController(IAccountService accountService)
        {
            _service = accountService;
        }
        
        /// <summary>
        /// Get specific account by unique id
        /// </summary>
        /// <param name="id">Account ID.</param>
        /// <returns>Account details.</returns>
        // GET: api/Accounts/0b4b7e2b-ffd1-4acf-81b3-e51d48155217
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Create a new account.
        /// </summary>
        /// <param name="request">Name of account holder.</param>
        /// <returns>Created account details.</returns>
        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Deposit funds into account
        /// </summary>
        /// <param name="id">Account ID.</param>
        /// <param name="amount">Deposit amount.</param>
        /// <returns>Updated account details.</returns>
        // POST: api/Accounts/0b4b7e2b-ffd1-4acf-81b3-e51d48155217/deposits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id}/deposits")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountResult<decimal>>> DepositAccount(Guid id, [FromBody] decimal amount)
        {
            var depositRequest = new TransactionRequest(Amount: amount, Id: id);
            var depositResult = await _service.Deposit(depositRequest);
            if (!depositResult.IsSuccess)
            {
                return BadRequest(depositResult.ErrorMessage);
            }

            return Ok(depositResult.Result);
        }
        
        /// <summary>
        /// Withdraws funds from an account.
        /// </summary>
        /// <param name="id">Account ID.</param>
        /// <param name="amount">Withdrawal amount.</param>
        /// <returns>Updated account details.</returns>
        // POST: api/Accounts/0b4b7e2b-ffd1-4acf-81b3-e51d48155217/withdrawals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id}/withdrawals")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountResult<decimal>>> WithdrawAccount(Guid id, [FromBody] decimal amount)
        {
            var withdrawRequest = new TransactionRequest(Amount: amount, Id: id);
            var withdrawResult = await _service.Withdraw(withdrawRequest);
            if (!withdrawResult.IsSuccess)
            {
                return BadRequest(new { Error = withdrawResult.ErrorMessage });
            }

            return Ok(withdrawResult.Result);
        }
        
        /// <summary>
        /// Transfers funds between two accounts.
        /// </summary>
        /// <param name="request">Amount to transfer, sender's ID, recipient's ID.</param>
        /// <returns>Updated account details.</returns>
        // POST: api/Accounts/transfers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("transfers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransferBalances>> Transfer(TransactionRequest request)
        {
            var result = await _service.Transfer(request);
            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.ErrorMessage });
            }

            return Ok(result.Result);
        }
    }
}
