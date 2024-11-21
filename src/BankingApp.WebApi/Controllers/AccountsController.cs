using System.Net;
using BankingApp.Application.DataAccess;
using BankingApp.Core;
using BankingApp.WebApi.ResponseBuilding;
using FluentValidation;
using IbanNet;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController(
    IResponseBuilder responseBuilder,
    IAccountsRepository accountsRepository,
    IIbanValidator ibanValidator,
    IValidator<CreateAccountRequest> createAccountRequestValidator) : ControllerBase
{
    [HttpGet("/{iban}")]
    public async Task<ActionResult> GetAsync([FromRoute] string iban, CancellationToken cancellationToken)
    {
        var validationResult = ibanValidator.Validate(iban);
        if (!validationResult.IsValid)
        {
            return responseBuilder.BuildValidationFailureResponse(
                [validationResult.Error!.ErrorMessage]);
        }
        
        var result = await accountsRepository.GetByIbanAsync(iban, cancellationToken);
        return responseBuilder.BuildResultResponse(result);
    }
    
    [HttpGet("/all")]
    public async Task<ActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await accountsRepository.GetAlLAsync(cancellationToken);
       return responseBuilder.BuildResultResponse(result);
    }

    [HttpPost("/new")]
    public async Task<ActionResult> CreateAsync(
        CreateAccountRequest createAccountRequest,
        CancellationToken cancellationToken)
    {
        var validationResult = await createAccountRequestValidator.ValidateAsync(createAccountRequest, cancellationToken);
        if (!validationResult.IsValid)
        {
            return responseBuilder.BuildValidationFailureResponse(validationResult.Errors);
        }
        
        var result = await accountsRepository.CreateAsync(createAccountRequest, cancellationToken);
        return responseBuilder.BuildResultResponse(result, HttpStatusCode.Created);
    }
}