using System.ComponentModel.DataAnnotations;

namespace BankingApp.Core;

/// <summary>
/// DepositAmountRequest. Used to either deposit or withdraw some amount from
/// an account.
/// </summary>
public class DepositRequest(
    string iban,
    decimal amount)
{
    /// <summary>
    /// An IBAN of a target account.
    /// </summary>
    /// <example>UA82753726N13TPJ1HH12VA660513</example>
    [Required]
    [MinLength(10)]
    [MaxLength(34)]
    public string Iban { get; init; } = iban;
    
    /// <summary>
    /// An amount to deposit/withdraw.
    /// </summary>
    /// <example>100.0</example>
    public decimal Amount { get; init; } = amount;
}