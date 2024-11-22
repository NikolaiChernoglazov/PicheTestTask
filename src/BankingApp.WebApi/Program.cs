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
builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();