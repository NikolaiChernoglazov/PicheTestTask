namespace BankingApp.Application;

public interface ILimitsProvider
{
     List<string> GetSupportedCurrencyCodes();

     decimal GetMaxAllowedAccountAmount();
     
     decimal GetMaxAllowedTransactionAmount();
}