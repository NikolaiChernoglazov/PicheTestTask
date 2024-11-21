namespace BankingApp.Core;

public record CreateAccountRequest(
    string Currency,
    decimal Amount);