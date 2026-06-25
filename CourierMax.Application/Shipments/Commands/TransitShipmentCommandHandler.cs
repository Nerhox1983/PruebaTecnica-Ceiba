using CourierMax.Domain.Interfaces;

namespace CourierMax.Application.Shipments.Commands
{
    public record TransitShipmentCommand(int ShipmentId, string UserId, string Reason);

    public class TransitShipmentCommandHandler
    {
        private readonly IShipmentRepository _shipmentRepository;

        public TransitShipmentCommandHandler(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        public async Task HandleAsync(TransitShipmentCommand command)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(command.ShipmentId);

            if (shipment == null)
            {
                throw new KeyNotFoundException($"No se encontró el envío con ID {command.ShipmentId}");
            }

            // Modifica el estado en memoria
            shipment.Transit(command.UserId, command.Reason);

            // Se apoya en el repositorio tal cual como en la creación
            await _shipmentRepository.UpdateAsync(shipment);
        }
    }
}