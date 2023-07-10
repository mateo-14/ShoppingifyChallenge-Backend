using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ShoppingifyChallenge.Data;
using ShoppingifyChallenge.Models;
using ShoppingifyChallenge.Models.Requests;
using ShoppingifyChallenge.Services;
using ShoppingifyChallenge.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
dataSourceBuilder.MapEnum<AuthProvider>();
dataSourceBuilder.MapEnum<ShoppingListStatus>();

var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<ShoppingListContext>(options => options.UseNpgsql(dataSource));
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true); // Lower case endpoints
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true); // Disable automatic model validation (I use FluentValidation instead)
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IValidator<MagicLinkRequest>, MagicLinkRequestValidator>();

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

public partial class Program { }