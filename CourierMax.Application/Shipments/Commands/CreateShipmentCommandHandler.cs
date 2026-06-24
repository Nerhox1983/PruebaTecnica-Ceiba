using CourierMax.Application.Shipments.DTOs;
using CourierMax.Domain.Entities;
using CourierMax.Domain.Interfaces;
using CourierMax.Domain.Services;

namespace CourierMax.Application.Shipments.Commands
{
    public class CreateShipmentCommandHandler
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly ISlaCalculator _slaCalculator;

        public CreateShipmentCommandHandler(IShipmentRepository shipmentRepository, ISlaCalculator slaCalculator)
        {
            _shipmentRepository = shipmentRepository;
            _slaCalculator = slaCalculator;
        }

        public async Task<ShipmentDto> HandleAsync(CreateShipmentCommand command)
        {            
            if (!Enum.TryParse(command.PackageType, true, out CourierMax.Domain.Enums.PackageType parsedPackageType))
                throw new Domain.Exceptions.BusinessException($"El tipo de paquete '{command.PackageType}' no es válido.");

            if (!Enum.TryParse(command.ServiceType, true, out CourierMax.Domain.Enums.ServiceType parsedServiceType))
                throw new Domain.Exceptions.BusinessException($"El tipo de servicio '{command.ServiceType}' no es válido.");
         
            var shipment = new Shipment(
                command.SenderName,
                command.SenderPhone,
                command.SenderAddress,
                command.ReceiverName,
                command.ReceiverPhone,
                command.ReceiverAddress,
                command.OriginCityId,
                command.DestinationCityId,
                command.WeightKg,
                command.LengthCm,
                command.WidthCm,
                command.HeightCm,
                parsedPackageType,
                parsedServiceType,
                command.Price
            );
            
            int attempts = 0;
            while (await _shipmentRepository.ExistsByTrackingCodeAsync(shipment.TrackingCode))
            {
                attempts++;
                if (attempts > 5)
                    throw new Domain.Exceptions.BusinessException("RN-05: No se pudo generar un código de rastreo único disponible.");

                shipment.RegenerateTrackingCode();
            }
            
            DateTime estimatedDelivery = _slaCalculator.CalculateEstimatedDeliveryDate(shipment.CreatedAt, parsedServiceType);
            
            shipment.EstimatedDeliveryDate = estimatedDelivery;            
            await _shipmentRepository.AddAsync(shipment);            
            return new ShipmentDto
            {
                Id = shipment.Id,
                TrackingCode = shipment.TrackingCode,
                SenderName = shipment.SenderName,
                SenderPhone = shipment.SenderPhone,
                SenderAddress = shipment.SenderAddress,
                ReceiverName = shipment.ReceiverName,
                ReceiverPhone = shipment.ReceiverPhone,
                DestinationAddress = shipment.ReceiverAddress,
                OriginCityId = shipment.OriginCityId,
                DestinationCityId = shipment.DestinationCityId,
                WeightKg = shipment.WeightKg,
                LengthCm = shipment.LengthCm,
                WidthCm = shipment.WidthCm,
                HeightCm = shipment.HeightCm,
                PackageType = shipment.PackageType.ToString(),
                ServiceType = shipment.ServiceType.ToString(),
                Price = shipment.TotalCost,
                Status = shipment.CurrentStatus.ToString(),
                CreatedAt = shipment.CreatedAt
            };
        }
    }
}