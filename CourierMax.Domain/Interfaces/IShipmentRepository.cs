using CourierMax.Domain.Entities;

namespace CourierMax.Domain.Interfaces
{
    public interface IShipmentRepository
    {
        Task<Shipment?> GetByIdAsync(int id);
        Task AddAsync(Shipment shipment);
        Task UpdateAsync(Shipment shipment);

        Task<bool> ExistsByTrackingCodeAsync(string trackingCode);
        
    }
}
