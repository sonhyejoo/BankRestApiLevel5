using System.Text.Json;
using BankRestApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BankRestApi.Models.DTOs;
using BankRestApi.Models.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Account = BankRestApi.Models.DTOs.Account;

namespace BankRestApi.Controllers;

/// <summary>
/// Controller for managing bank interactions.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[ApiController]
[Produces("application/json")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _service;

    public AccountsController(IAccountService accountService)
    {
        _service = accountService;
    }

    /// <summary>
    /// Get list of accounts;
    /// Filter by name, sort by "name" or "balance", set sorting order to descending by desc=true,
    /// set page size and page number
    /// </summary>
    /// <returns>List of accounts</returns>
    /// GET: api/Accounts
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Account>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
    public async Task<ActionResult<IEnumerable<Account>>> GetAccounts(
        string? name,
        string sortBy = "",
        bool desc = false,
        int pageNumber = 1,
        int pageSize = 5)
    {
        if (pageNumber < 0)
        {
            pageNumber = 1;
        }
        
        pageSize = pageSize switch
        {
            < 0 => 5,
            > 32 => 32,
            _ => pageSize
        };

        var queryParameters = new GetAccountsQueryParameters(name, sortBy, desc, pageNumber, pageSize);
        
        var result = await _service.GetAccounts(queryParameters);
        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.Result.PageData));
        
        return result.IsSuccess
            ? Ok(result.Result.Accounts)
            : StatusCode((int)result.StatusCode!, result.ErrorMessage);
    }
    
    /// <summary>
    /// Get specific account by unique id
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <returns>Account details</returns>
    // GET: api/Accounts/0b4b7e2b-ffd1-4acf-81b3-e51d48155217
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(Account))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(string))]
    public async Task<ActionResult<Account>> GetAccount(Guid id)
    {
        var request = new GetAccount(id);
        var result = await _service.Get(request);

        return result.IsSuccess
            ? Ok(result.Result)
            : StatusCode((int)result.StatusCode!, result.ErrorMessage);
    }

    /// <summary>
    /// Create a new account
    /// </summary>
    /// <param name="request">Name of account holder</param>
    /// <returns>Created account details</returns>
    // POST: api/Accounts
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type=typeof(Account))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(string))]
    public async Task<ActionResult<Account>> CreateAccount(CreateAccount request)
    {
        var result = await _service.Create(request);
        var createdAccount = result.Result;

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAccount), new { id = createdAccount!.Id }, createdAccount)
            : StatusCode((int)result.StatusCode!, result.ErrorMessage);
    }

    /// <summary>
    /// Deposit funds into account
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="request">Deposit amount</param>
    /// <returns>Updated account details</returns>
    // POST: api/Accounts/0b4b7e2b-ffd1-4acf-81b3-e51d48155217/deposits
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost("{id}/deposits")]
    [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(Account))]
    [ProducesResponseType(StatusCodes.Status400BadRequest,  Type=typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(string))]
    public async Task<ActionResult<Account>> DepositAccount(Guid id, TransactionRequest request)
    {
        var transaction = new Transaction(request.Amount, id);
        var result = await _service.Deposit(transaction);
        
        return result.IsSuccess
            ? Ok(result.Result)
            : StatusCode((int)result.StatusCode!, result.ErrorMessage);
    }

    /// <summary>
    /// Withdraws funds from an account
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="request">Withdrawal amount</param>
    /// <returns>Updated account details</returns>
    // POST: api/Accounts/0b4b7e2b-ffd1-4acf-81b3-e51d48155217/withdrawals
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost("{id}/withdrawals")]
    [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(Account))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(string))]
    public async Task<ActionResult<Account>> WithdrawAccount(Guid id, TransactionRequest request)
    {
        var transaction = new Transaction(request.Amount, id);
        var result = await _service.Withdraw(transaction);
        
        return result.IsSuccess
            ? Ok(result.Result)
            : StatusCode((int)result.StatusCode!, result.ErrorMessage);
    }
    
    /// <summary>
    /// Transfers funds between two accounts
    /// </summary>
    /// <param name="request">Amount to transfer, sender's ID, recipient's ID</param>
    /// <returns>Updated accounts' balances</returns>
    // POST: api/Accounts/transfers
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost("transfers")]
    [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(Account))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(string))]
    public async Task<ActionResult<TransferDetails>> Transfer(Transaction request)
    {
        var result = await _service.Transfer(request);
        
        return result.IsSuccess
            ? Ok(result.Result)
            : StatusCode((int)result.StatusCode!, result.ErrorMessage);
    }
    
    /// <summary>
    /// Converts account balance value to other currencies
    /// </summary>
    /// <param name="id">Account ID to convert balance of</param>
    /// <param name="request">List of comma separated currency codes</param>
    /// <returns>Account information and balance in specified currencies (all if not specified)</returns>
    [HttpPost("{id}/conversion")]
    [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(ConvertedBalances))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type=typeof(string))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type=typeof(string))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type=typeof(string))]
    public async Task<ActionResult<ConvertedBalances>> GetConvertedBalances(Guid id, ConvertRequest request)
    {
        var convertCommand = new ConvertCommand(id, request.Currencies);
        var result = await _service.ConvertBalances(convertCommand);
        
        return result.IsSuccess
            ? Ok(result.Result)
            : StatusCode((int)result.StatusCode!, result.ErrorMessage);
    }
}
