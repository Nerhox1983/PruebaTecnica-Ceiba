using CourierMax.Domain.Entities;
using CourierMax.Domain.Interfaces;
using CourierMax.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CourierMax.Infrastructure.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly CourierMaxContext _context;

        public ShipmentRepository(CourierMaxContext context)
        {
            _context = context;
        }

        public async Task<Shipment?> GetByIdAsync(int id)
        {
            return await _context.Shipments.FindAsync(id);
        }

        public async Task AddAsync(Shipment shipment)
        {
            if (string.IsNullOrEmpty(shipment.TrackingCode))
            {                
                string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 3).ToUpper();                
                shipment.TrackingCode = $"TRK-{uniqueId}";
            }
            await _context.Shipments.AddAsync(shipment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Shipment shipment)
        {
            _context.Shipments.Update(shipment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByTrackingCodeAsync(string trackingCode)
        {
            return await _context.Shipments.AnyAsync(s => s.TrackingCode == trackingCode);
        }
    }
}