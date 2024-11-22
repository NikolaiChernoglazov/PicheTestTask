using BankingApp.Application;
using BankingApp.Core;
using FluentValidation;

namespace BankingApp.WebApi.Validation;

public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountRequestValidator(ILimitsProvider limitsProvider)
    {
        var supportedCurrencies =
            limitsProvider.GetSupportedCurrencyCodes();
        var amountLimit = limitsProvider.GetMaxAllowedAccountAmount();
        RuleFor(r => r.Currency.ToUpper())
            .NotEmpty()
            .Must(supportedCurrencies.Contains)
            .WithMessage("Currency {PropertyValue} is either invalid or not supported. " +
                $"Supported currencies are: {string.Join(", ", supportedCurrencies)}");
        RuleFor(r => r.Amount).InclusiveBetween(
            0m, amountLimit);
    }
}