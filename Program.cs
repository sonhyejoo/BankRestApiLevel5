using BankRestApi.Interfaces;
using BankRestApi.Models;
using BankRestApi.Repositories;
using BankRestApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AccountContext>(opt =>
    opt.UseInMemoryDatabase("AccountsList"));
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddHttpClient<IExchangeService, ExchangeService>(client =>
    client.BaseAddress = new Uri(builder.Configuration["ExchangeService:BaseAddress"]));

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