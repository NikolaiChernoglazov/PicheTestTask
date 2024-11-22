using BankingApp.Core;
using ErrorOr;

namespace BankingApp.Application;

public interface IAccountManager
{
    Task<ErrorOr<Account>> GetByIbanAsync(string iban,
        CancellationToken cancellationToken);
    
    Task<ErrorOr<List<Account>>> GetAlLAsync(CancellationToken cancellationToken);
    
    Task<ErrorOr<Account>> CreateAsync(CreateAccountRequest createAccountRequest,
        CancellationToken cancellationToken);
    
    Task<ErrorOr<Account>> DepositAsync(DepositRequest request, CancellationToken cancellationToken);
    
    Task<ErrorOr<Account>> WithdrawAsync(DepositRequest request, CancellationToken cancellationToken);
    
    Task<ErrorOr<List<Account>>> TransferAsync(TransferRequest request, CancellationToken cancellationToken);
}