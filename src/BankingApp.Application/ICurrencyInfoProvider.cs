namespace BankingApp.Application;

public interface ICurrencyInfoProvider
{
     List<string> GetSupportedCurrencyCodes(); 
}