using CourierMax.Application.Shipments.Commands;
using CourierMax.Application.Shipments.Queries;
using CourierMax.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

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
        }

        /// <summary>
        /// Crea un nuevo envío en el sistema aplicando validaciones de FluentValidation.
        /// </summary>
        /// <param name="command">Los datos necesarios para la creación del envío.</param>
        /// <param name="handler">El manejador encargado de procesar la creación del envío.</param>
        /// <param name="validator">El validador de FluentValidation inyectado desde la capa de Application.</param>
        /// <returns>Un resultado HTTP 201 Created con el envío creado o 400/500 en caso de error.</returns>
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
        /// <param name="command">Los datos necesarios para la asignación del vehículo.</param>
        /// <param name="handler">El manejador encargado de procesar la asignación.</param>
        /// <returns>Un resultado HTTP 200 OK con el DTO actualizado, 404 si no existe o 400 por error de negocio.</returns>
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
        /// <param name="id">El identificador único del envío a consultar.</param>
        /// <param name="queryHandler">Manejador encargado de procesar la consulta y obtener los logs desde la persistencia.</param>
        /// <returns>Un resultado HTTP 200 con la lista o 404 si no registra auditorías.</returns>
        private static async Task<IResult> GetShipmentHistoryAsync(
            int id,
            CourierMax.Application.Shipments.Queries.GetShipmentHistoryQueryHandler queryHandler)
        {
            var history = await queryHandler.HandleAsync(id);

            if (history == null || !history.Any())
            {
                return Results.NotFound(new { message = $"No se encontró historial para el envío con ID {id}" });
            }

            return Results.Ok(history);
        }

        /// <summary>
        /// Consulta los envíos que presentan retrasos respecto a su SLA dentro de un rango de fechas.
        /// </summary>
        private static async Task<IResult> GetDelayedShipmentsAsync(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            CourierMax.Application.Shipments.Queries.GetDelayedShipmentsQueryHandler queryHandler)
        {
            if (startDate > endDate)
            {
                return Results.BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha de fin." });
            }

            var query = new GetDelayedShipmentsQuery(startDate, endDate);
            var delayedShipments = await queryHandler.HandleAsync(query);

            return Results.Ok(delayedShipments);
        }
    }
}