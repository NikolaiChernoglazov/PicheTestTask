namespace BankingApp.Core;

public record Account(
    int Id,
    string Iban,
    string Currency,
    decimal Amount,
    DateTimeOffset CreatedAt);