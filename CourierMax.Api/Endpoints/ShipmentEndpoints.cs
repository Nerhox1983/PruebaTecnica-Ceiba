using CourierMax.Application.Shipments.Commands;
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
            
        }

        /// <summary>
        /// Crea un nuevo envío en el sistema.
        /// </summary>
        /// <param name="command">Los datos necesarios para la creación del envío.</param>
        /// <param name="handler">El manejador encargado de procesar la creación del envío.</param>
        /// <returns>Un resultado HTTP 201 Created con el envío creado o 400/500 en caso de error.</returns>
        private static async Task<IResult> CreateShipmentAsync(
            CreateShipmentCommand command,
            CreateShipmentCommandHandler handler)
        {
            try
            {
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
    }
}
