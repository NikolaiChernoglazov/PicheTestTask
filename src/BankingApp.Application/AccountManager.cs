using BankingApp.Application.DataAccess;
using BankingApp.Core;
using ErrorOr;

namespace BankingApp.Application;

public class AccountManager(
    IAccountsRepository accountsRepository,
    IIbanProvider ibanGenerator,
    ILimitsProvider limitsProvider) : IAccountManager
{
    public Task<ErrorOr<Account>> GetByIbanAsync(string iban, CancellationToken cancellationToken)
    {
        return accountsRepository.GetByIbanAsync(iban, cancellationToken);
    }

    public Task<ErrorOr<List<Account>>> GetAlLAsync(CancellationToken cancellationToken)
    {
        return accountsRepository.GetAlLAsync( cancellationToken);
    }

    public Task<ErrorOr<Account>> CreateAsync(CreateAccountRequest createAccountRequest, CancellationToken cancellationToken)
    {
        var entity = new Account(0,
            ibanGenerator.Generate("UA"),
            createAccountRequest.Currency,
            createAccountRequest.Amount,
            DateTimeOffset.Now);
        return accountsRepository.AddOrUpdateAsync(entity, cancellationToken);
    }

    public async Task<ErrorOr<Account>> DepositAsync(DepositRequest request, CancellationToken cancellationToken)
    {
        var accountResult = await accountsRepository.GetByIbanAsync(request.Iban, cancellationToken);
        if (accountResult.IsError)
        {
            return accountResult;
        }
        
        var account = accountResult.Value;
        var accountLimit = limitsProvider.GetMaxAllowedAccountAmount();

        if (account.Amount + request.Amount > accountLimit)
        {
            return Error.Forbidden(description: $"The account amount must not exceed {accountLimit}.");
        }
        
        return await accountsRepository.AddOrUpdateAsync(account with
        {
            Amount = account.Amount + request.Amount,
        }, cancellationToken);
    }

    public async Task<ErrorOr<Account>> WithdrawAsync(DepositRequest request, CancellationToken cancellationToken)
    {
        var accountResult = await accountsRepository.GetByIbanAsync(request.Iban, cancellationToken);
        if (accountResult.IsError)
        {
            return accountResult;
        }
        
        var account = accountResult.Value;
 
        if (account.Amount < request.Amount)
        {
            return Error.Forbidden(description: $"This account does not have enough amount to withdraw.");
        }
        
        return await accountsRepository.AddOrUpdateAsync(account with
        {
            Amount = account.Amount - request.Amount,
        }, cancellationToken);
    }

    public async Task<ErrorOr<List<Account>>> TransferAsync(TransferRequest request, CancellationToken cancellationToken)
    {
        var fromAccountResult = await accountsRepository.GetByIbanAsync(request.FromIban, cancellationToken);
        if (fromAccountResult.IsError)
        {
            return fromAccountResult.Errors;
        }
        var fromAccount = fromAccountResult.Value;
        if (fromAccount.Amount < request.Amount)
        {
            return Error.Forbidden(description: $"This account does not have enough amount to withdraw.");
        }
        
        var toAccountResult = await accountsRepository.GetByIbanAsync(request.ToIban, cancellationToken);
        if (toAccountResult.IsError)
        {
            return toAccountResult.Errors;
        }
        var toAccount = toAccountResult.Value;

        if (toAccount.Currency != fromAccount.Currency)
        {
            // In production, we can implement currency exhange logic, requesting exchange rates from some external API 
            return Error.Forbidden(description: "Transfer between accounts with different currencies is not supported.");
        }
        
        var accountLimit = limitsProvider.GetMaxAllowedAccountAmount();
        if (toAccount.Amount + request.Amount > accountLimit)
        {
            return Error.Forbidden(description: $"The account amount must not exceed {accountLimit}.");
        }
        
        return await accountsRepository.UpdateManyAsync([
            fromAccount with
            {
                Amount = fromAccount.Amount - request.Amount,
            },
            toAccount with
            {
                Amount = toAccount.Amount + request.Amount,
            }], cancellationToken);
    }
}