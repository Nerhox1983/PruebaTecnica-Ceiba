using CourierMax.Application.Shipments.DTOs;
using CourierMax.Domain.Interfaces;

namespace CourierMax.Application.Shipments.Commands
{
    public class AssignVehicleCommandHandler
    {
        private readonly IShipmentRepository _shipmentRepository;

        public AssignVehicleCommandHandler(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        public async Task<ShipmentDto> HandleAsync(AssignVehicleCommand command)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(command.ShipmentId);
            if (shipment == null)
            {
                throw new KeyNotFoundException($"No se encontró el envío con ID {command.ShipmentId}");
            }

            shipment.AssignVehicle(command.LicensePlate, command.UserId);

            await _shipmentRepository.UpdateAsync(shipment);

            return new ShipmentDto
            {
                Id = shipment.Id,
                SenderName = shipment.SenderName,
                SenderPhone = shipment.SenderPhone,
                ReceiverName = shipment.ReceiverName,
                ReceiverPhone = shipment.ReceiverPhone,
                ReceiverAddress = shipment.ReceiverAddress,
                DestinationAddress = shipment.ReceiverAddress,
                DestinationCityId = shipment.DestinationCityId,
                WeightKg = shipment.WeightKg,
                Price = shipment.TotalCost,
                Status = shipment.CurrentStatus.ToString(),
                AssignedVehiclePlate = shipment.VehicleId,
                CreatedAt = shipment.CreatedAt
            };
        }
    }
}