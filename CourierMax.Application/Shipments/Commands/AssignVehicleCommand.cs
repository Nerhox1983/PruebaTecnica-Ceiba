namespace CourierMax.Application.Shipments.Commands
{
    public class AssignVehicleCommand
    {
        public int ShipmentId { get; set; }
        public int VehicleId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}