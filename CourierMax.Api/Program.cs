using CourierMax.Api.Endpoints;
using CourierMax.Application.Shipments.Commands;
using CourierMax.Domain.Interfaces;
using CourierMax.Infrastructure.Persistence;
using CourierMax.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CourierMaxContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();

builder.Services.AddScoped<CreateShipmentCommandHandler>();
builder.Services.AddScoped<AssignVehicleCommandHandler>();
builder.Services.AddScoped<CourierMax.Application.Shipments.Queries.GetShipmentHistoryQueryHandler>();

builder.Services.AddScoped<CourierMax.Domain.Services.ISlaCalculator, CourierMax.Domain.Services.SlaCalculator>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapShipmentEndpoints();

app.Run();