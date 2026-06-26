using CourierMax.Application.Shipments.DTOs;
using CourierMax.Application.Shipments.Interfaces;
using CourierMax.Domain.Enums;
using CourierMax.Domain.Interfaces;

namespace CourierMax.Infrastructure.Services
{
    public class ShipmentQueryService : IShipmentQueryService
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public ShipmentQueryService(IShipmentRepository shipmentRepository, IVehicleRepository vehicleRepository)
        {
            _shipmentRepository = shipmentRepository;
            _vehicleRepository = vehicleRepository;
        }

        public async Task<DriverEfficiencyReportDto> GetDriverEfficiencyReportAsync(string driverId, DateTime startDate, DateTime endDate)
        {            
            var vehicles = await _vehicleRepository.GetByDriverOrPlateAsync(driverId);
            var vehicleIds = vehicles.Select(v => v.VehicleId).ToList();
            
            var allShipments = await _shipmentRepository.GetDelayedShipmentsAsync(startDate, endDate);
            
            var filtered = allShipments.Where(s => s.VehicleId.HasValue && vehicleIds.Contains(s.VehicleId.Value)).ToList();

            var totalAssigned = filtered.Count;

            var totalDelivered = filtered.Count(s => s.CurrentStatus == ShipmentStatus.ENTREGADO);
            var totalCanceled = filtered.Count(s => s.CurrentStatus == ShipmentStatus.CANCELADO);
            var totalInTransit = filtered.Count(s => s.CurrentStatus == ShipmentStatus.EN_TRANSITO);
            var totalWeight = filtered.Sum(s => s.WeightKg);

            return new DriverEfficiencyReportDto
            {
                DriverId = driverId,
                TotalAssigned = totalAssigned,
                TotalDelivered = totalDelivered,
                TotalCanceled = totalCanceled,
                TotalInTransit = totalInTransit,
                TotalWeightTransported = totalWeight
            };
        }
    }
}