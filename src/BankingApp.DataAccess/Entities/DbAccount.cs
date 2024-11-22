using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankingApp.Core;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.DataAccess.Entities;

[Table("Account")]
public class DbAccount
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Iban { get; set; }
    
    [Required]
    public string Currency { get; set; }
    
    [Required]
    [Precision(14, 4)]
    public decimal Amount { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    
    public Account ToCoreEntity() => new Account(
        Id, Iban, Currency, Amount, CreatedAt);

    public static DbAccount FromCoreEntity(Account account)
        => new DbAccount
        {
            Id = account.Id, Iban = account.Iban, Currency = account.Currency, Amount = account.Amount,
            CreatedAt = account.CreatedAt
        };
}