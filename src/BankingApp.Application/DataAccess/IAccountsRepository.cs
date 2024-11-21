using BankingApp.Core;
using ErrorOr;

namespace BankingApp.Application.DataAccess;

public interface IAccountsRepository
{
    Task<ErrorOr<Account>> GetByIbanAsync(string iban,
        CancellationToken cancellationToken);
    
    Task<ErrorOr<List<Account>>> GetAlLAsync(CancellationToken cancellationToken);
    
    Task<ErrorOr<Account>> CreateAsync(CreateAccountRequest createAccountRequest,
        CancellationToken cancellationToken);
}