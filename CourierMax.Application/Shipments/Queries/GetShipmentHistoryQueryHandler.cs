using CourierMax.Application.Shipments.DTOs;
using CourierMax.Domain.Interfaces;

namespace CourierMax.Application.Shipments.Queries
{
    public class GetShipmentHistoryQueryHandler
    {
        private readonly IShipmentRepository _shipmentRepository;
        public GetShipmentHistoryQueryHandler(IShipmentRepository shipmentRepository) 
        {
            _shipmentRepository = shipmentRepository;
        }

        public async Task<List<ShipmentStatusHistoryDto>> HandleAsync(int shipmentId)        
        {
            var history = await _shipmentRepository.GetStatusHistoryAsync(shipmentId);
            return history.Select(h => new ShipmentStatusHistoryDto
            {
                Id = h.Id,
                PreviousStatus = h.PreviousStatus.ToString(),
                NewStatus = h.NewStatus.ToString(),
                ChangedAt = h.ChangedAt,
                UserId = h.UserId,
                ChangeReason = h.ChangeReason,
            }).ToList();
        }
    }
}
