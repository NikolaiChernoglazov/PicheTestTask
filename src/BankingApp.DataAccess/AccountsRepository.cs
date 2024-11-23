using BankingApp.Application.DataAccess;
using BankingApp.Core;
using BankingApp.DataAccess.Entities;
using ErrorOr;
using IbanNet.Registry;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.DataAccess;

public class AccountsRepository(
    BankingDbContext dbContext) : IAccountsRepository
{
    public async Task<ErrorOr<Account>> GetByIbanAsync(string iban,
        CancellationToken cancellationToken)
    {
        try
        {
            var dbEntity = await dbContext.Accounts.FirstOrDefaultAsync(
                x => x.Iban == iban.ToUpper(), cancellationToken);
            if (dbEntity is null)
            {
                return Error.NotFound(description: $"Account with IBAN {iban} was not found");
            }

            var entity = dbEntity.ToCoreEntity();
            dbContext.Entry(dbEntity).State = EntityState.Detached;

            return entity;
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

    public async Task<ErrorOr<Account>> AddOrUpdateAsync(Account entity, CancellationToken cancellationToken)
    {
        try
        {
            var dbEntity = DbAccount.FromCoreEntity(entity);
            dbContext.Accounts.Update(dbEntity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return dbEntity.ToCoreEntity();
        }
        catch (Exception e)
        {
            return Error.Unexpected(description: e.Message);
        }
    }

    public async Task<ErrorOr<List<Account>>> UpdateManyAsync(
        List<Account> accounts, CancellationToken cancellationToken)
    {
        try
        {
            var dbEntities = accounts.Select(DbAccount.FromCoreEntity).ToList();
            dbContext.Accounts.UpdateRange(dbEntities);
            // SaveChanges has a transaction inside, so we don't worry about that
            await dbContext.SaveChangesAsync(cancellationToken);
            return dbEntities.Select(e => e.ToCoreEntity()).ToList();
        }
        catch (Exception e)
        {
            return Error.Unexpected(description: e.Message);
        }
    }
}