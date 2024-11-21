namespace BankingApp.Core;

public record Account(
    string Iban,
    string Currency,
    decimal Amount,
    DateTimeOffset CreatedAt);