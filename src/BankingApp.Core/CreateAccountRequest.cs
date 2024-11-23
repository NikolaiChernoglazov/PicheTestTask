using System.ComponentModel.DataAnnotations;

namespace BankingApp.Core;

/// <summary>
/// CreateAccountRequest
/// </summary>
public record CreateAccountRequest(
    string Currency,
    decimal Amount)
{
    /// <summary>
    /// Code of currency to open an account in.
    /// </summary>
    /// <example>USD</example>
    [Required]
    public string Currency { get; init; } = Currency;
    
    /// <summary>
    /// The initial amount.
    /// </summary>
    /// <example>100.0</example>
    public decimal Amount { get; init; } = Amount;
}