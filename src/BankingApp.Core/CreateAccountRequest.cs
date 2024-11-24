using System.ComponentModel.DataAnnotations;

namespace BankingApp.Core;

/// <summary>
/// CreateAccountRequest
/// </summary>
public class CreateAccountRequest(
    string currency,
    decimal amount)
{
    /// <summary>
    /// Code of currency to open an account in.
    /// </summary>
    /// <example>USD</example>
    [Required]
    public string Currency { get; init; } = currency;
    
    /// <summary>
    /// The initial amount.
    /// </summary>
    /// <example>100.0</example>
    public decimal Amount { get; init; } = amount;
}