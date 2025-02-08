using FrameUp.OrderService.Api.Extensions;
using System.Text.Json.Serialization;
using FrameUp.OrderService.Api.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var settings = builder.Configuration.GetSection("Settings").Get<Settings>()!;
builder.Services.AddSingleton(settings);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services
    .AddAuthorization(options =>
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    })
    .AddHttpContextAccessor();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "FrameUp.OrderService.Api", Version = "v1" });
        options.AddAuthorizationOptions();
    });

builder.Services
    .AddAuthenticationExtension(settings);

builder.Services.AddLogBee(settings);

builder.AddCustomHealthChecks(settings);

// Add services to the container.
builder.Services
    .AddMassTransit(settings)
    .AddRepositories()
    .AddDatabaseContext(settings)
    .AddMinIO(settings)
    .AddServices()
    .AddUseCases();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomHealthChecks();

app.UseAuthentication();

// Use CORS middleware
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.UseLogBee();

app.Run();

// We should get "Program" class from the main project but it automatically references Microsoft.AspNetCore.Mvc.Testing.Program class
// https://stackoverflow.com/questions/55131379/integration-testing-asp-net-core-with-net-framework-cant-find-deps-json
public partial class Program { }