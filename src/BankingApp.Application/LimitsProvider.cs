namespace BankingApp.Application;

public class LimitsProvider : ILimitsProvider
{
    // For simplicity, let's just hardcode these values.
    // In production, we might read them from a configuration file and have more complicated logic
  
    public List<string> GetSupportedCurrencyCodes()
    {
        return ["USD", "EUR", "UAH"];
    }

    public decimal GetMaxAllowedAccountAmount()
        => 1_000_000_000_000m;
    
    public decimal GetMaxAllowedTransactionAmount()
        => 100_000_000m;
}