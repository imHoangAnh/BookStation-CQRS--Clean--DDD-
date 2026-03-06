using System.Reflection;
using BookStation.Application;
using BookStation.Infrastructure;
using BookStation.Query.Queries.Books;
using BookStation.WebApi.Extensions;
using MediatR;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger wtih JWT support
builder.Services.AddSwaggerWithJwt();

// Add JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

// MediatR: register Query handlers (read flow) in addition to Application (command) handlers.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(GetAllBooksQuery).Assembly,
    typeof(BookStation.Application.DependencyInjection).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())    // testing environment - run only in development
{
    app.UseSwagger();                   // Swagger middleware: Creates endpoint JSON 
    app.UseSwaggerUI(
        options =>{
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BookStation API");
        options.RoutePrefix = string.Empty; // Swagger UI at root URL
        }
    );
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();