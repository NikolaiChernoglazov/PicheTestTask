using BankingApp.Application;
using BankingApp.Core;
using FluentValidation;
using IbanNet;

namespace BankingApp.WebApi.Validation;

public class DepositRequestValidator : AbstractValidator<DepositRequest>
{
    public DepositRequestValidator(
        IIbanValidator ibanValidator,
        ILimitsProvider limitsProvider)
    {
        var transactionLimit = limitsProvider.GetMaxAllowedTransactionAmount();
        RuleFor(r => r.Iban).MustBeValidIban(ibanValidator);
        RuleFor(r => r.Amount).ExclusiveBetween(0m, transactionLimit);
    }
}