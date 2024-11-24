using IbanNet.Registry;

namespace BankingApp.Application;

// Proxy needed for unit testing
public class IbanProvider : IIbanProvider
{
    private static readonly IbanGenerator IbanGenerator = new();
    
    public string Generate(string country)
    {
        return IbanGenerator.Generate(country).ToString();
    }
}