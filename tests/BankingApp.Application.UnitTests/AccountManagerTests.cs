using BankingApp.Application.DataAccess;
using BankingApp.Core;
using ErrorOr;
using FluentAssertions;
using Moq;

namespace BankingApp.Application.UnitTests;

public class AccountManagerTests
{
    private readonly Mock<IAccountsRepository> _accountsRepositoryMock;
    private readonly Mock<IIbanProvider> _ibanGeneratorMock;
    private readonly Mock<ILimitsProvider> _limitsProviderMock;
    private readonly IAccountManager _accountManager;

    public AccountManagerTests()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);
        
        _accountsRepositoryMock = mockRepository.Create<IAccountsRepository>();
        _ibanGeneratorMock = mockRepository.Create<IIbanProvider>();
        _limitsProviderMock = mockRepository.Create<ILimitsProvider>();
        
        _accountManager = new AccountManager(
            _accountsRepositoryMock.Object,
            _ibanGeneratorMock.Object,
            _limitsProviderMock.Object);
    }

    
    [Fact]
    public async Task GetByIbanAsync_ShouldReturnResultFromRepository()
    {
        // Arrange
        var iban = "UA123";
        var account = new Account(1, iban, "USD", 100, DateTimeOffset.Now);
        _accountsRepositoryMock.Setup(repo => repo.GetByIbanAsync(iban, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        var result = await _accountManager.GetByIbanAsync(iban, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(account, result.Value);
    }
    
    
    [Fact]
    public async Task CreateAsync_ShouldReturnResultFromRepository()
    {
        // Arrange
        var createAccountRequest = new CreateAccountRequest("USD", 100);
        var generatedIban = "UA123";
        var expectedAccount = new Account(1, generatedIban, "USD", 100, DateTimeOffset.Now);
        _ibanGeneratorMock.Setup(generator => generator.Generate("UA")).Returns(generatedIban);
        _accountsRepositoryMock.Setup(repo => repo.AddOrUpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAccount);

        // Act
        var result = await _accountManager.CreateAsync(createAccountRequest, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal(generatedIban, result.Value.Iban);
        Assert.Equal(createAccountRequest.Currency, result.Value.Currency);
        Assert.Equal(createAccountRequest.Amount, result.Value.Amount);
    }
    
    
    [Fact]
    public async Task DepositAsync_ShouldReturnUpdatedAccount_WhenRequestIsValid()
    {
        var request = new DepositRequest("VALID_IBAN", 50.0m);
        var account = new Account(1, request.Iban, "USD", 100.0m, DateTimeOffset.UtcNow);

        _accountsRepositoryMock.Setup(repo => repo.GetByIbanAsync(request.Iban, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _limitsProviderMock.Setup(lp => lp.GetMaxAllowedAccountAmount())
            .Returns(200.0m);
        _accountsRepositoryMock.Setup(repo => repo.AddOrUpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(account with { Amount = account.Amount + request.Amount });

        var result = await _accountManager.DepositAsync(request, CancellationToken.None);

        Assert.False(result.IsError);
        Assert.Equal(150.0m, result.Value.Amount);
    }

    [Fact]
    public async Task DepositAsync_ShouldReturnError_WhenAccountNotFound()
    {
        var request = new DepositRequest("NON_EXISTENT_IBAN", 50.0m);
        ErrorOr<Account> error = Error.NotFound(description: "NON_EXISTENT_IBAN");
        
        _accountsRepositoryMock.Setup(repo => repo.GetByIbanAsync(request.Iban, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound(description: "NON_EXISTENT_IBAN"));

        var result = await _accountManager.DepositAsync(request, CancellationToken.None);

        Assert.True(result.IsError);
        result.Errors.Should().Contain(e => e.Type == ErrorType.NotFound);
    }

    [Fact]
    public async Task DepositAsync_ShouldReturnError_WhenDepositExceedsLimit()
    {
        var request = new DepositRequest("VALID_IBAN", 150.0m);
        var account = new Account(1, request.Iban, "USD", 100.0m, DateTimeOffset.UtcNow);

        _accountsRepositoryMock.Setup(repo => repo.GetByIbanAsync(request.Iban, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _limitsProviderMock.Setup(lp => lp.GetMaxAllowedAccountAmount())
            .Returns(200.0m);

        var result = await _accountManager.DepositAsync(request, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Contains(result.Errors, error => error.Type == ErrorType.Forbidden);
    }
    
    
    [Fact]
    public async Task WithdrawAsync_ShouldReturnError_WhenAccountNotFound()
    {
        // Arrange
        var request = new DepositRequest("InvalidIban", 100);
        _accountsRepositoryMock.Setup(repo => repo.GetByIbanAsync(request.Iban, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.NotFound("Account not found"));

        // Act
        var result = await _accountManager.WithdrawAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, error => error.Type == ErrorType.NotFound);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldReturnError_WhenInsufficientFunds()
    {
        // Arrange
        var account = new Account(1, "ValidIban", "USD", 50, DateTimeOffset.Now);
        var request = new DepositRequest("ValidIban", 100);
        _accountsRepositoryMock.Setup(repo => repo.GetByIbanAsync(request.Iban, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Act
        var result = await _accountManager.WithdrawAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, error => error.Description.Contains("not have enough amount"));
    }
    
    [Fact]
    public async Task WithdrawAsync_ShouldReturnUpdatedAccount_WhenSuccessful()
    {
        // Arrange
        var account = new Account(1, "ValidIban", "USD", 200, DateTimeOffset.Now);
        var request = new DepositRequest("ValidIban", 100);
        var updatedAccount = account with { Amount = account.Amount - request.Amount };
        
        _accountsRepositoryMock.Setup(repo => repo.GetByIbanAsync(request.Iban, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _accountsRepositoryMock.Setup(repo => repo.AddOrUpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedAccount);

        // Act
        var result = await _accountManager.WithdrawAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(updatedAccount.Amount, result.Value.Amount);
    }
    
    
    [Fact]
    public async Task TransferAsync_ShouldReturnError_WhenInsufficientFunds()
    {
        // Arrange
        var transferRequest = new TransferRequest("FROM_IBAN", "TO_IBAN", 100m);

        _accountsRepositoryMock.Setup(r => r.GetByIbanAsync("FROM_IBAN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Error.Forbidden(description: "Insufficient funds"));

        // Act
        var result = await _accountManager.TransferAsync(transferRequest, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Description == "Insufficient funds");
    }

    [Fact]
    public async Task TransferAsync_ShouldReturnError_WhenDifferentCurrencies()
    {
        // Arrange
        var transferRequest = new TransferRequest("FROM_IBAN", "TO_IBAN", 100m);
        var fromAccount = new Account(1, "FROM_IBAN", "USD", 200m, DateTimeOffset.Now);
        var toAccount = new Account(2, "TO_IBAN", "EUR", 100m, DateTimeOffset.Now);

        _accountsRepositoryMock.Setup(r => r.GetByIbanAsync("FROM_IBAN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(fromAccount);

        _accountsRepositoryMock.Setup(r => r.GetByIbanAsync("TO_IBAN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(toAccount);

        // Act
        var result = await _accountManager.TransferAsync(transferRequest, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Description == "Transfer between accounts with different currencies is not supported.");
    }

    [Fact]
    public async Task TransferAsync_ShouldReturnError_WhenExceedsAccountLimit()
    {
        // Arrange
        var transferRequest = new TransferRequest("FROM_IBAN", "TO_IBAN", 100m);
        var fromAccount = new Account(1, "FROM_IBAN", "USD", 200m, DateTimeOffset.Now);
        var toAccount = new Account(2, "TO_IBAN", "USD", 450m, DateTimeOffset.Now);

        _accountsRepositoryMock.Setup(r => r.GetByIbanAsync("FROM_IBAN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(fromAccount);

        _accountsRepositoryMock.Setup(r => r.GetByIbanAsync("TO_IBAN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(toAccount);

        _limitsProviderMock.Setup(p => p.GetMaxAllowedAccountAmount()).Returns(500m);

        // Act
        var result = await _accountManager.TransferAsync(transferRequest, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(result.Errors, e => e.Description == "The account amount must not exceed 500.");
    }

    [Fact]
    public async Task TransferAsync_ShouldReturnUpdatedAccounts_WheSuccess()
    {
        // Arrange
        var transferRequest = new TransferRequest("FROM_IBAN", "TO_IBAN", 50m);
        var fromAccount = new Account(1, "FROM_IBAN", "USD", 200m, DateTimeOffset.Now);
        var toAccount = new Account(2, "TO_IBAN", "USD", 100m, DateTimeOffset.Now);

        _accountsRepositoryMock.Setup(r => r.GetByIbanAsync("FROM_IBAN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(fromAccount);

        _accountsRepositoryMock.Setup(r => r.GetByIbanAsync("TO_IBAN", It.IsAny<CancellationToken>()))
            .ReturnsAsync(toAccount);

        _limitsProviderMock.Setup(p => p.GetMaxAllowedAccountAmount()).Returns(500m);

        _accountsRepositoryMock.Setup(r => r.UpdateManyAsync(It.IsAny<List<Account>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Account>
            {
                fromAccount with { Amount = 150m },
                toAccount with { Amount = 150m }
            });

        // Act
        var result = await _accountManager.TransferAsync(transferRequest, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        var accounts = result.Value;
        Assert.Equal(2, accounts.Count);
        Assert.Equal(150m, accounts.First(a => a.Iban == "FROM_IBAN").Amount);
        Assert.Equal(150m, accounts.First(a => a.Iban == "TO_IBAN").Amount);
    }
}