using Microsoft.AspNetCore.Mvc;
using BankRestApi.Models.DTOs;
using BankRestApi.Services;
using Account = BankRestApi.Models.DTOs.Account;

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
        /// <returns>Account details if successful, otherwise returns BadRequest.</returns>
        // GET: api/Accounts/0b4b7e2b-ffd1-4acf-81b3-e51d48155217
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Account>> GetAccount(Guid id)
        {
            var request = new GetAccountRequest(id);
            var result = await _service.Get(request);

            if (!result.IsSuccess && result.ErrorMessage.StartsWith("No"))
            {
                return NotFound(new { Error = result.ErrorMessage });
            }
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.ErrorMessage });
            }

            return Ok(result.Result);
        }

        /// <summary>
        /// Create a new account.
        /// </summary>
        /// <param name="request">Name of account holder.</param>
        /// <returns>Created account details, otherwise BadRequest if name is empty.</returns>
        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Account>> CreateAccount(CreateAccountRequest request)
        {
            var result = await _service.Create(request);
            var success = result.IsSuccess;
            if (!success)
            {
                return BadRequest(new { Error = result.ErrorMessage });
            }
            var createdAccount = result.Result;

            return CreatedAtAction(nameof(GetAccount), new { id = createdAccount.Id }, createdAccount);
        }

        /// <summary>
        /// Deposit funds into account
        /// </summary>
        /// <param name="id">Account ID.</param>
        /// <param name="amount">Deposit amount.</param>
        /// <returns>Updated account details if successful, otherwise BadRequest.</returns>
        // POST: api/Accounts/0b4b7e2b-ffd1-4acf-81b3-e51d48155217/deposits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id}/deposits")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountResult<Account>>> DepositAccount(Guid id, [FromBody] decimal amount)
        {
            var request = new TransactionRequest(Amount: amount, Id: id);
            var result = await _service.Deposit(request);
            if (!result.IsSuccess && result.ErrorMessage.StartsWith("No"))
            {
                return NotFound(new { Error = result.ErrorMessage });
            }
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.ErrorMessage });
            }

            return Ok(result.Result);
        }
        
        /// <summary>
        /// Withdraws funds from an account.
        /// </summary>
        /// <param name="id">Account ID.</param>
        /// <param name="amount">Withdrawal amount.</param>
        /// <returns>Updated account details if successful, otherwise BadRequest.</returns>
        // POST: api/Accounts/0b4b7e2b-ffd1-4acf-81b3-e51d48155217/withdrawals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id}/withdrawals")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountResult<Account>>> WithdrawAccount(Guid id, [FromBody] decimal amount)
        {
            var request = new TransactionRequest(Amount: amount, Id: id);
            var result = await _service.Withdraw(request);
            if (!result.IsSuccess && result.ErrorMessage.StartsWith("No"))
            {
                return NotFound(new { Error = result.ErrorMessage });
            }
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.ErrorMessage });
            }

            return Ok(result.Result);
        }
        
        /// <summary>
        /// Transfers funds between two accounts.
        /// </summary>
        /// <param name="request">Amount to transfer, sender's ID, recipient's ID.</param>
        /// <returns>Updated accounts' balances if successful, otherwise BadRequest.
        /// </returns>
        // POST: api/Accounts/transfers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("transfers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransferDetails>> Transfer(TransactionRequest request)
        {
            var result = await _service.Transfer(request);
            if (!result.IsSuccess && result.ErrorMessage.StartsWith("No"))
            {
                return NotFound(new { Error = result.ErrorMessage });
            }
            
            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.ErrorMessage });
            }

            return Ok(result.Result);
        }
    }
}
