using CourierMax.Application.Shipments.DTOs;
using CourierMax.Domain.Interfaces;

namespace CourierMax.Application.Shipments.Queries
{
    public class GetDelayedShipmentsQueryHandler
    {
        private readonly IShipmentRepository _repository;

        public GetDelayedShipmentsQueryHandler(IShipmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ShipmentDto>> HandleAsync(GetDelayedShipmentsQuery query)
        {            
            var shipments = await _repository.GetDelayedShipmentsAsync(query.StartDate, query.EndDate);
            
            return shipments.Select(s => new ShipmentDto
            {
                Id = s.Id,
                TrackingCode = s.TrackingCode,
                SenderName = s.SenderName,
                ReceiverName = s.ReceiverName,
                Status = s.CurrentStatus.ToString(),
                DeliveredAt = s.EstimatedDeliveryDate,
                CreatedAt = s.CreatedAt                
            });
        }
    }
}
