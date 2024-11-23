using System.ComponentModel.DataAnnotations;

namespace BankingApp.Core;

/// <summary>
/// TransferRequest
/// </summary>
public class TransferRequest(
    string fromIban,
    string toIban,
    decimal amount)
{
    /// <summary>
    /// An IBAN of an account to transit from.
    /// </summary>
    /// <example>UA82753726N13TPJ1HH12VA660513</example>
    [Required]
    [MinLength(10)]
    [MaxLength(34)]
    public string FromIban { get; init; } = fromIban;
    
    /// <summary>
    /// An IBAN of an account to transit to.
    /// </summary>
    /// <example>UA93071818EE721J12G4KQAKHAW1T</example>
    [Required]
    [MinLength(10)]
    [MaxLength(34)]
    public string ToIban { get; init; } = toIban;
    
    /// <summary>
    /// An amount to transfer.
    /// </summary>
    /// <example>100.0</example>
    public decimal Amount { get; init; } = amount;
}
