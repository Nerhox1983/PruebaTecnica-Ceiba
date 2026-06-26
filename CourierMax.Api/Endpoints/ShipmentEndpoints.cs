using CourierMax.Application.Shipments.Commands;
using CourierMax.Application.Shipments.DTOs;
using CourierMax.Application.Shipments.Queries;
using CourierMax.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourierMax.Api.Endpoints
{
    public static class ShipmentEndpoints
    {
        /// <summary>
        /// Registra las rutas para la gestión de envíos.
        /// </summary>
        /// <param name="app">La interfaz del constructor de rutas de endpoints.</param>
        public static void MapShipmentEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/shipments")
                           .WithTags("Shipments");

            group.MapPost("/", CreateShipmentAsync);

            group.MapGet("/{id:int}/history", GetShipmentHistoryAsync)
                 .WithName("GetShipmentHistory")
                 .WithOpenApi();

            group.MapPost("/assign-vehicle", AssignVehicleAsync).WithOpenApi();

            group.MapGet("/delayed", GetDelayedShipmentsAsync)
                 .WithName("GetDelayedShipments")
                 .WithOpenApi();

            group.MapGet("/drivers/{driverId}/efficiency", GetDriverEfficiencyAsync)
                 .WithName("GetDriverEfficiency")
                 .WithOpenApi();
        }

        /// <summary>
        /// Crea un nuevo envío en el sistema aplicando validaciones de FluentValidation.
        /// </summary>
        private static async Task<IResult> CreateShipmentAsync(
            CreateShipmentCommand command,
            CreateShipmentCommandHandler handler,
            IValidator<CreateShipmentCommand> validator)
        {
            try
            {
                var validationResult = await validator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    var errores = validationResult.Errors.Select(e => new {
                        Campo = e.PropertyName,
                        Error = e.ErrorMessage
                    });

                    return Results.BadRequest(new
                    {
                        message = "Error de validación en la solicitud.",
                        errors = errores
                    });
                }

                var result = await handler.HandleAsync(command);
                return Results.Created($"/api/shipments/{result.Id}", result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Error interno", detail = ex.Message }, statusCode: 500);
            }
        }

        /// <summary>
        /// Asigna un vehículo a un envío registrando el cambio de estado (RF-02).
        /// </summary>
        private static async Task<IResult> AssignVehicleAsync(
            AssignVehicleCommand command,
            AssignVehicleCommandHandler handler)
        {
            try
            {
                var result = await handler.HandleAsync(command);
                return Results.Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
            catch (BusinessException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Json(new { message = "Error interno", detail = ex.Message }, statusCode: 500);
            }
        }

        /// <summary>
        /// Obtiene el historial cronológico de cambios de estado y auditoría de un envío específico (RF-02).
        /// </summary>
        private static async Task<IResult> GetShipmentHistoryAsync(
            int id,
            GetShipmentHistoryQueryHandler queryHandler)
        {
            var history = await queryHandler.HandleAsync(id);

            if (history == null || !history.Any())
            {
                return Results.NotFound(new { message = $"No se encontró historial para el envío con ID {id}" });
            }

            return Results.Ok(history);
        }

        /// <summary>
        /// Consulta los envíos que presentan retrasos respecto a su SLA dentro de un rango de fechas (RF-05).
        /// </summary>
        private static async Task<IResult> GetDelayedShipmentsAsync(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            GetDelayedShipmentsQueryHandler queryHandler)
        {
            if (startDate > endDate)
            {
                return Results.BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha de fin." });
            }

            var query = new GetDelayedShipmentsQuery(startDate, endDate);
            var delayedShipments = await queryHandler.HandleAsync(query);

            return Results.Ok(delayedShipments);
        }

        /// <summary>
        /// Genera el reporte de métricas de eficiencia por conductor en un rango de fechas (RF-06).
        /// </summary>
        private static async Task<IResult> GetDriverEfficiencyAsync(
            string driverId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromServices] GetDriverEfficiencyQueryHandler queryHandler)
        {
            if (startDate > endDate)
            {
                return Results.BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha de fin." });
            }

            var query = new GetDriverEfficiencyQuery(driverId, startDate, endDate);
            var report = await queryHandler.HandleAsync(query);

            return Results.Ok(report);
        }
    }
}