namespace BankingApp.Application;

public interface IIbanProvider
{
    string Generate(string country);
}