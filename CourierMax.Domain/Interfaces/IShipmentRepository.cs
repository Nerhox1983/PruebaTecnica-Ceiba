using CourierMax.Domain.Entities;

namespace CourierMax.Domain.Interfaces
{
    public interface IShipmentRepository
    {
        Task<Shipment?> GetByIdAsync(int id);
        Task AddAsync(Shipment shipment);
        Task UpdateAsync(Shipment shipment);
        Task<bool> ExistsByTrackingCodeAsync(string trackingCode);
        Task<List<ShipmentStatusHistory>> GetStatusHistoryAsync(int shipmentId);
        Task<IEnumerable<Shipment>> GetDelayedShipmentsAsync(DateTime startDate, DateTime endDate);
        //Task<DriverEfficiencyReportDto> GetDriverEfficiencyReportAsync(string driverId, DateTime startDate, DateTime endDate);
    }
}
