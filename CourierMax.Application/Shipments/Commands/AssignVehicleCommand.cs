namespace CourierMax.Application.Shipments.Commands
{
    public record AssignVehicleCommand(
        int ShipmentId,
        string LicensePlate
    );
}