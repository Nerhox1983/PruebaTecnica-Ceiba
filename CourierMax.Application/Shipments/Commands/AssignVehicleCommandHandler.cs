using CourierMax.Application.Shipments.DTOs;
using CourierMax.Domain.Exceptions;
using CourierMax.Domain.Interfaces;

namespace CourierMax.Application.Shipments.Commands
{
    public class AssignVehicleCommandHandler
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public AssignVehicleCommandHandler(IShipmentRepository shipmentRepository, IVehicleRepository vehicleRepository)
        {
            _shipmentRepository = shipmentRepository;
            _vehicleRepository = vehicleRepository;
        }

        public async Task<ShipmentDto> HandleAsync(AssignVehicleCommand command)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(command.ShipmentId);
            if (shipment == null)
            {
                throw new KeyNotFoundException($"No se encontró el envío con ID {command.ShipmentId}");
            }

            var vehicle = await _vehicleRepository.GetByIdAsync(command.VehicleId);
            if (vehicle == null)
            {
                throw new BusinessException($"El vehículo con ID {command.VehicleId} no está registrado en el sistema.");
            }

            shipment.AssignVehicle(vehicle.VehicleId, vehicle.MaxWeightKg, vehicle.MaxVolumeM3, vehicle.IsActive, command.UserId);

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
                AssignedVehiclePlate = vehicle.LicensePlate,
                CreatedAt = shipment.CreatedAt
            };
        }
    }
}