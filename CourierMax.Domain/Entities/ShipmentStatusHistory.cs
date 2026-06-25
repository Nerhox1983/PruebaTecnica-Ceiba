using CourierMax.Domain.Enums;

namespace CourierMax.Domain.Entities
{
    public class ShipmentStatusHistory
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public ShipmentStatus PreviousStatus { get; set; }
        public ShipmentStatus NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }
        public string UserId { get; set; }
        public string? ChangeReason { get; set; }

        public ShipmentStatusHistory() 
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
