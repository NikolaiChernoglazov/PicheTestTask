using Microsoft.AspNetCore.Mvc;

namespace BankingApp.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    [HttpGet("/all")]
    public ActionResult ListAccounts(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}