using FrameUp.OrderService.Api.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    }); ;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Add services to the container.
builder.Services
    .AddMassTransit(builder.Configuration)
    .AddRepositories()
    .AddDatabaseContext(builder.Configuration)
    .AddUseCases();

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

// We should get "Program" class from the main project but it automatically references Microsoft.AspNetCore.Mvc.Testing.Program class
// https://stackoverflow.com/questions/55131379/integration-testing-asp-net-core-with-net-framework-cant-find-deps-json
public partial class Program { }