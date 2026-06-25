using CourierMax.Domain.Interfaces;

namespace CourierMax.Application.Shipments.Commands
{
    public record DeliverShipmentCommand(int ShipmentId, string UserId, string Reason);

    public class DeliverShipmentCommandHandler
    {
        private readonly IShipmentRepository _shipmentRepository;

        public DeliverShipmentCommandHandler(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        public async Task HandleAsync(DeliverShipmentCommand command)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(command.ShipmentId);

            if (shipment == null)
            {
                throw new KeyNotFoundException($"No se encontró el envío con ID {command.ShipmentId}");
            }

            shipment.Deliver(command.UserId, command.Reason);

            await _shipmentRepository.UpdateAsync(shipment);
        }
    }
}