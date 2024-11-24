namespace BankingApp.Application.UnitTests;

public class LimitsProviderTests
{
    private readonly LimitsProvider _limitsProvider = new();

    [Fact]
    public void GetSupportedCurrencyCodes_ShouldReturnSupportedCurrencies()
    {
        // Act
        var supportedCurrencies = _limitsProvider.GetSupportedCurrencyCodes();

        // Assert
        Assert.Contains("USD", supportedCurrencies);
        Assert.Contains("EUR", supportedCurrencies);
        Assert.Contains("UAH", supportedCurrencies);
    }

    [Fact]
    public void GetMaxAllowedAccountAmount_ShouldReturnMaxValue()
    {
        // Act
        var maxAllowed = _limitsProvider.GetMaxAllowedAccountAmount();

        // Assert
        Assert.Equal(1_000_000_000_000m, maxAllowed);
    }

    [Fact]
    public void GetMaxAllowedTransactionAmount_ShouldReturnMaxValue()
    {
        // Act
        var maxAllowed = _limitsProvider.GetMaxAllowedTransactionAmount();

        // Assert
        Assert.Equal(100_000_000m, maxAllowed);
    }
}