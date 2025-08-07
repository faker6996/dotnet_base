using System.Data;
using Npgsql;
using DOTNET_BASE.APPLICATION.User;
using DOTNET_BASE.APPLICATION.Account;
using DOTNET_BASE.CORE.Interfaces;
using DOTNET_BASE.INFRASTRUCTURE.Repositories;
using DOTNET_BASE.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=dotnet_base_db;Username=postgres;Password=postgres;";

builder.Services.AddScoped<IDbConnection>(provider => new NpgsqlConnection(connectionString));

// Repository registration
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// Service registration
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// Middleware registration
builder.Services.AddCustomMiddleware(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Custom middleware pipeline
app.UseCustomMiddlewarePipeline(builder.Configuration);

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
