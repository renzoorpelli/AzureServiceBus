using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entidades.DataContext;
using Entidades.Domain;
using FlightApi.Middlewares;
using DataAccess.Generic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//agrego los servicios GenericRepo y UnitOfWork
IoC.AddDependency(builder.Services);

var connectionString = builder.Configuration.GetConnectionString("ApplicationDB");

builder.Services.AddDbContext<AzureServiceBusContext>(options =>
{
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/", async (Vuelo bodyFromRequest, IUnitOfWork context, IGenericRepository<Vuelo> repository ) =>
{
    await repository.CreateAsync(bodyFromRequest);

    context.Commit();

    return Results.Created($"/vuelo/{bodyFromRequest.NumeroVuelo}", bodyFromRequest);
});

app.Run();
