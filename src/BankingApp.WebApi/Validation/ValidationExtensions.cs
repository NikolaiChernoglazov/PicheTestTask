using FluentValidation;
using IbanNet;

namespace BankingApp.WebApi.Validation;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeValidIban<T>(
        this IRuleBuilder<T, string> ruleBuilder, IIbanValidator ibanValidator)
    {
        return ruleBuilder
            .NotEmpty()
            .Must<T, string>((_, iban, context) =>
            {
                var result = ibanValidator.Validate(iban);
                if (result.IsValid)
                    return true;
                context.MessageFormatter.AppendArgument(
                    "IBAN validation error", result.Error!.ErrorMessage);
                return false;
            })
            .WithMessage("{IBAN validation error}");
    }
    
}