namespace BankingApp.Core;

public record DepositRequest(
    string Iban,
    decimal Amount);