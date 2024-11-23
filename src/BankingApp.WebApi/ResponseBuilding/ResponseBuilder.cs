using System.Net;
using ErrorOr;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.WebApi.ResponseBuilding;

public class ResponseBuilder : IResponseBuilder
{
    private static readonly Dictionary<ErrorType, HttpStatusCode>
        StatusCodeMapping = new()
        {
            { ErrorType.NotFound, HttpStatusCode.NotFound },
            { ErrorType.Forbidden, HttpStatusCode.Forbidden },
            { ErrorType.Unexpected, HttpStatusCode.InternalServerError }
        };
    
    public ActionResult BuildResultResponse<TValue>(ErrorOr<TValue> result, 
        HttpStatusCode successStatusCode = HttpStatusCode.OK)
    {
        return result.MatchFirst(
            value => new ObjectResult(result.Value)
            {
                StatusCode = (int)successStatusCode
            },
            BuildErrorResponse);
    }

    public ActionResult BuildValidationFailureResponse(List<ValidationFailure> failures)
    {
        return BuildValidationFailureResponse(failures
            .Select(f => f.ErrorMessage).ToList());
    }

    public ActionResult BuildValidationFailureResponse(List<string> errors)
    {
        return new BadRequestObjectResult(errors);
    }

    private static ObjectResult BuildErrorResponse(Error error)
    {
        return new ObjectResult(
            new
            {
                Error = error.Description,
            })
        {
            StatusCode = StatusCodeMapping.TryGetValue(error.Type, out var value) 
                ? (int)value
                : (int)HttpStatusCode.InternalServerError
        };
    }
}