using CourierMax.Domain.Entities;
using CourierMax.Domain.Enums;
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
            try
            {
                return await _context.Shipments.FindAsync(id);
            }
            catch (InvalidOperationException)
            {
                var rawShipment = await _context.Shipments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (rawShipment == null) return null;

                return await _context.Shipments.FirstOrDefaultAsync(s => s.Id == id);
            }
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

        public async Task<List<ShipmentStatusHistory>> GetStatusHistoryAsync(int shipmentId)
        {
            return await _context.ShipmentStatusHistories
                .Where(h => h.ShipmentId == shipmentId)
                .OrderBy(h => h.ChangedAt)
                .ToListAsync();
        }
    }
}