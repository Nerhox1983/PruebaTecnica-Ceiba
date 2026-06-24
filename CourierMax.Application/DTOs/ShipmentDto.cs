namespace CourierMax.Application.Shipments.DTOs
{
    public class ShipmentDto
    {
        public int Id { get; set; }
        public string TrackingCode { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderPhone { get; set; } = string.Empty;
        public string SenderAddress { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string ReceiverPhone { get; set; } = string.Empty;
        public string ReceiverAddress { get; set; } = string.Empty;
        public string DestinationAddress { get; set; } = string.Empty;
        public int OriginCityId { get; set; }
        public int DestinationCityId { get; set; }
        public decimal WeightKg { get; set; }
        public decimal LengthCm { get; set; }
        public decimal WidthCm { get; set; }
        public decimal HeightCm { get; set; }
        public string PackageType { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? AssignedVehiclePlate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}