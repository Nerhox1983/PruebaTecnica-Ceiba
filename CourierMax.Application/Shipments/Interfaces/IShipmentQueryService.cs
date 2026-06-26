using CourierMax.Application.Shipments.DTOs;

namespace CourierMax.Application.Shipments.Interfaces
{
    public interface IShipmentQueryService
    {
        Task<DriverEfficiencyReportDto> GetDriverEfficiencyReportAsync(string driverId, DateTime startDate, DateTime endDate);
    }
}
