namespace CourierMax.Application.Shipments.DTOs
{
    public class ShipmentStatusHistoryDto
    {
        public int Id { get; set; }
        public string PreviousStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? ChangeReason { get; set; }
    }
}
