using BankingApp.Application.DataAccess;
using BankingApp.Core;
using BankingApp.DataAccess.Entities;
using ErrorOr;
using IbanNet.Registry;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.DataAccess;

public class AccountsRepository(
    BankingDbContext dbContext,
    IIbanGenerator ibanGenerator) : IAccountsRepository
{
    public async Task<ErrorOr<Account>> GetByIbanAsync(string iban,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await dbContext.Accounts.FirstOrDefaultAsync(
                x => x.Iban == iban.ToUpper(), cancellationToken);
            if (entity is null)
            {
                return Error.NotFound($"Account with IBAN {iban} is not found");
            }

            return entity.ToCoreEntity();
        }
        catch (Exception e)
        {
            return Error.Unexpected(description: e.Message);
        }
    }

    public async Task<ErrorOr<List<Account>>> GetAlLAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await dbContext.Accounts
                .Select(dba => dba.ToCoreEntity())
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            return Error.Unexpected(description: e.Message);
        }
    }

    public async Task<ErrorOr<Account>> CreateAsync(CreateAccountRequest createAccountRequest, CancellationToken cancellationToken)
    {
        try
        {
            var entity = new DbAccount
            {
                // Probably it is more correct to generate it
                // at the Application layer, but let's do it here for simplicity
                Iban = ibanGenerator.Generate("UA").ToString(),
                Currency = createAccountRequest.Currency,
                CreatedAt = DateTimeOffset.Now,
            };
            dbContext.Accounts.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return entity.ToCoreEntity();
        }
        catch (Exception e)
        {
            return Error.Unexpected(description: e.Message);
        }
    }
}