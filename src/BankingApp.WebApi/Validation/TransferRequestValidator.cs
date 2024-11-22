using BankingApp.Application;
using BankingApp.Core;
using FluentValidation;
using IbanNet;

namespace BankingApp.WebApi.Validation;

public class TransferRequestValidator : AbstractValidator<TransferRequest>
{
    public TransferRequestValidator(IIbanValidator ibanValidator,
        ILimitsProvider limitsProvider)
    {
        var transactionLimit = limitsProvider.GetMaxAllowedTransactionAmount();
        RuleFor(r => r.FromIban).MustBeValidIban(ibanValidator);
        RuleFor(r => r.ToIban).MustBeValidIban(ibanValidator);
        RuleFor(r => r.Amount).ExclusiveBetween(0m, transactionLimit);
    }
}