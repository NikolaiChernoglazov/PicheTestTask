using BankingApp.Application;
using BankingApp.Core;
using FluentValidation;

namespace BankingApp.WebApi.Validation;

public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountRequestValidator(ICurrencyInfoProvider currencyInfoProvider)
    {
        var supportedCurrencies =
            currencyInfoProvider.GetSupportedCurrencyCodes();
        RuleFor(r => r.Currency.ToUpper())
            .NotEmpty()
            .Must(supportedCurrencies.Contains)
            .WithMessage("Currency {PropertyValue} is either invalid or not supported. " +
                $"Supported currencies are: {string.Join(", ", supportedCurrencies)}");
        RuleFor(r => r.Amount).InclusiveBetween(
            0m, 1000000000000m);
    }
}