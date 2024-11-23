using System.Reflection;
using BankingApp.Application;
using BankingApp.Application.DataAccess;
using BankingApp.Core;
using BankingApp.DataAccess;
using BankingApp.WebApi.ResponseBuilding;
using BankingApp.WebApi.Validation;
using FluentValidation;
using IbanNet;
using IbanNet.Registry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var modelsAssembly = typeof(CreateAccountRequest).Assembly;
    c.IncludeXmlComments(GetXmlDocumentationFileFor(modelsAssembly));
});

var services = builder.Services;
services.AddScoped<IResponseBuilder, ResponseBuilder>();
services.AddScoped<IAccountsRepository, AccountsRepository>();
services.AddScoped<ILimitsProvider, LimitsProvider>();
services.AddScoped<IValidator<CreateAccountRequest>, CreateAccountRequestValidator>();
services.AddScoped<IValidator<DepositRequest>, DepositRequestValidator>();
services.AddScoped<IValidator<TransferRequest>, TransferRequestValidator>();
services.AddScoped<IIbanGenerator, IbanGenerator>();
services.AddScoped<IIbanValidator, IbanValidator>();
services.AddScoped<IAccountManager, AccountManager>();

services.AddDbContext<BankingDbContext>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
return;


string GetXmlDocumentationFileFor(Assembly assembly)
{
    var documentationFile = $"{assembly.GetName().Name}.xml";
    var path = Path.Combine(AppContext.BaseDirectory, documentationFile);

    return path;
}