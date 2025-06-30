using BankRestApi.Models;

namespace BankRestApi.Services;

public class AccountService
{
    private readonly AccountContext _context;

    public AccountService(AccountContext context)
    {
        _context = context;
    }
}