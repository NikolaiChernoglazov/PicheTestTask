namespace BankingApp.Core;

public record TransferRequest(
    string FromIban,
    string ToIban,
    decimal Amount);
