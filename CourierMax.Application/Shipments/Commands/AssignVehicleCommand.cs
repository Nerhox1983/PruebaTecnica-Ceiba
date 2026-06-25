namespace CourierMax.Application.Shipments.Commands
{
    public class AssignVehicleCommand
    {
        public int ShipmentId { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}