using System.Net;
using BankingApp.Application;
using BankingApp.Core;
using BankingApp.WebApi.Controllers;
using BankingApp.WebApi.ResponseBuilding;
using FluentValidation;
using IbanNet;
using Moq;
using ErrorOr;
using FluentAssertions;
using FluentValidation.Results;
using IbanNet.Registry;
using IbanNet.Validation.Results;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.WebApi.UnitTests.Controllers;

public class AccountsControllerTests
{
    private readonly Mock<IResponseBuilder> _responseBuilder;
    private readonly Mock<IAccountManager> _accountManager;
    private readonly Mock<IIbanValidator> _ibanValidator;
    private readonly Mock<IValidator<CreateAccountRequest>> _createAccountRequestValidator;
    private readonly AccountsController _accountsController;

    public AccountsControllerTests()
    {
        var mockRepository = new MockRepository(MockBehavior.Strict);
        _responseBuilder = mockRepository.Create<IResponseBuilder>();
        _accountManager = mockRepository.Create<IAccountManager>();
        _ibanValidator = mockRepository.Create<IIbanValidator>();
        _createAccountRequestValidator = mockRepository.Create<IValidator<CreateAccountRequest>>();
        
        _accountsController = new AccountsController(
            _responseBuilder.Object, _accountManager.Object, _ibanValidator.Object, _createAccountRequestValidator.Object);
    }
    
    
    [Fact]
    public async Task GetAllAsync_GetsResultFromRepositoryAndBuildsResponse()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        ErrorOr<List<Account>> result = new List<Account>();
        var expectedResponse = new OkObjectResult(result.Value);
        
        _accountManager.Setup(ar => ar.GetAlLAsync(cancellationToken))
            .ReturnsAsync(result);
        _responseBuilder.Setup(r => r.BuildResultResponse(result, HttpStatusCode.OK))
            .Returns(expectedResponse);
        
        // Act
        var actualResponse = await _accountsController.GetAllAsync(cancellationToken);
        
        // Assert
        actualResponse.Should().BeSameAs(expectedResponse);
    }
    
    [Fact]
    public async Task CreateAsync_ValidationFails_BuildsErrorResponse()
    {
        // Arrange
        var request = new CreateAccountRequest("", 0);
        var cancellationToken = CancellationToken.None;
        var validationError = "Currency is required";
        var validationResult = new FluentValidation.Results.ValidationResult()
        {
            Errors = [new ValidationFailure(
                "Currency", validationError)]
        };
        
        var expectedResponse = new BadRequestObjectResult(validationError);
        
        _createAccountRequestValidator.Setup(
                r => r.ValidateAsync(request, cancellationToken))
            .ReturnsAsync(validationResult);
        _responseBuilder.Setup(r => r.BuildValidationFailureResponse(validationResult.Errors))
            .Returns(expectedResponse);
        
        // Act
        var actualResponse = await _accountsController.CreateAsync(request, cancellationToken);
        
        // Assert
        actualResponse.Should().BeSameAs(expectedResponse);
    }
    
    [Fact]
    public async Task CreateAsync_ValidationSuceeds_GetsResultFromRepositoryAndBuildsResponse()
    {
        // Arrange
        var request = new CreateAccountRequest("USD", 0);
        var cancellationToken = CancellationToken.None;
        var validationResult = new FluentValidation.Results.ValidationResult();
        ErrorOr<Account> result = 
            new Account(1, string.Empty, string.Empty, 0, DateTimeOffset.MinValue);
        var expectedResponse = new OkObjectResult(result.Value);
        
        _createAccountRequestValidator.Setup(
            v => v.ValidateAsync(request, cancellationToken))
            .ReturnsAsync(validationResult);
        _accountManager.Setup(ar => ar.CreateAsync(request, cancellationToken))
            .ReturnsAsync(result);
        _responseBuilder.Setup(r => r.BuildResultResponse(result, HttpStatusCode.Created))
            .Returns(expectedResponse);
        
        // Act
        var actualResponse = await _accountsController.CreateAsync(request, cancellationToken);
        
        // Assert
        actualResponse.Should().BeSameAs(expectedResponse);
    }
    
    
    [Fact]
    public async Task GetAsync_ValidationFails_BuildsErrorResponse()
    {
        // Arrange
        var request = "";
        var cancellationToken = CancellationToken.None;
        var validationError = "IBAN is required";
        var validationResult = new IbanNet.ValidationResult
        {
            Error = new ErrorResult(validationError)
        };
        
        var expectedResponse = new BadRequestObjectResult(validationError);
        
        _ibanValidator.Setup(
                v => v.Validate(request))
            .Returns(validationResult);
        _responseBuilder.Setup(r => r.BuildValidationFailureResponse(
            It.Is<List<string>>(s => s.Contains(validationError))))
            .Returns(expectedResponse);
        
        // Act
        var actualResponse = await _accountsController.GetAsync(request, cancellationToken);
        
        // Assert
        actualResponse.Should().BeSameAs(expectedResponse);
    }
    
    [Fact]
    public async Task GetAsync_ValidationSuceeds_GetsResultFromRepositoryAndBuildsResponse()
    {
        // Arrange
        var request = new IbanGenerator().Generate("UA").ToString();
        var cancellationToken = CancellationToken.None;
        var validationResult = new IbanNet.ValidationResult();
        ErrorOr<Account> result = 
            new Account(1, string.Empty, string.Empty, 0, DateTimeOffset.MinValue);
        var expectedResponse = new OkObjectResult(result.Value);
        
        _ibanValidator.Setup(v => v.Validate(request))
            .Returns(validationResult);
        _accountManager.Setup(ar => ar.GetByIbanAsync(request, cancellationToken))
            .ReturnsAsync(result);
        _responseBuilder.Setup(r => r.BuildResultResponse(result, HttpStatusCode.OK))
            .Returns(expectedResponse);
        
        // Act
        var actualResponse = await _accountsController.GetAsync(request, cancellationToken);
        
        // Assert
        actualResponse.Should().BeSameAs(expectedResponse);
    }
}