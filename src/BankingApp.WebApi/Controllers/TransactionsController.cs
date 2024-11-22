using BankingApp.Application;
using BankingApp.Core;
using BankingApp.WebApi.ResponseBuilding;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionsController(
    IResponseBuilder responseBuilder,
    IValidator<DepositRequest> depositValidator,
    IValidator<TransferRequest> transferValidator,
    IAccountManager accountManager) : ControllerBase
{
    // Note: will return core entity instead of special viewmodel entity to save time.
    // In production could use ViewModel and AutoMapper library
    
    [HttpPost("/deposit")]
    public async Task<ActionResult> DepositAsync(
        DepositRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await depositValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return responseBuilder.BuildValidationFailureResponse(validationResult.Errors);
        }
        
        var result = await accountManager.DepositAsync(request, cancellationToken);
        return responseBuilder.BuildResultResponse(result);
    }
    
    [HttpPost("/withdraw")]
    public async Task<ActionResult> WithdrawAsync(
        DepositRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await depositValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return responseBuilder.BuildValidationFailureResponse(validationResult.Errors);
        }
        
        var result = await accountManager.WithdrawAsync(request, cancellationToken);
        return responseBuilder.BuildResultResponse(result);
    }
    
    [HttpPost("/transfer")]
    public async Task<ActionResult> TransferAsync(
        TransferRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await transferValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return responseBuilder.BuildValidationFailureResponse(validationResult.Errors);
        }
        
        var result = await accountManager.TransferAsync(request, cancellationToken);
        return responseBuilder.BuildResultResponse(result);
    }
}