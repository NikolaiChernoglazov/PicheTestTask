namespace BankingApp.Application;

public class CurrencyInfoProvider : ICurrencyInfoProvider
{
    public List<string> GetSupportedCurrencyCodes()
    {
        // For simplicity, let's just hardcode supported currency codes.
        // In production, we might read them from a configuration file
        // We don't read them from database because we might not support all the currencies present there
        return ["USD", "EUR", "UAH"];
    }
}