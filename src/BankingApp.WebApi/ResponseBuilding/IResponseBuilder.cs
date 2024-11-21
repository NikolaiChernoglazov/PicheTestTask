using System.Net;
using ErrorOr;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.WebApi.ResponseBuilding;

public interface IResponseBuilder
{
    ActionResult BuildResultResponse<TValue>(ErrorOr<TValue> result,
        HttpStatusCode successStatusCode = HttpStatusCode.OK);
    
    ActionResult BuildValidationFailureResponse(List<ValidationFailure> failures);
    
    ActionResult BuildValidationFailureResponse(List<string> errors);
}