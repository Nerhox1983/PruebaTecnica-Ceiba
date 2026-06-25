using CourierMax.Domain.Enums;

namespace CourierMax.Domain.Entities
{
    public class ShipmentStatusHistory
    {
        public int Id { get; private set; }
        public int ShipmentId { get; private set; }
        public ShipmentStatus PreviousStatus { get; private set; }
        public ShipmentStatus NewStatus { get; private set; }
        public DateTime ChangedAt { get; private set; }
        public string UserId { get; private set; }
        public string? ChangeReason { get; private set; }

        private ShipmentStatusHistory() 
        {
        }

        public ShipmentStatusHistory(ShipmentStatus previousStatus, 
            ShipmentStatus newStatus, 
            string changedByUserId,
            string? reason = null)
        {
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
            ChangedAt = DateTime.UtcNow;
            UserId = changedByUserId;
            ChangeReason = reason;
        }
    }
}
