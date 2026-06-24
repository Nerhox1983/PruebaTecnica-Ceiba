namespace CourierMax.Application.Shipments.Commands
{    
    public record CreateShipmentCommand(
        string SenderName,
        string SenderPhone,
        string SenderAddress,
        string ReceiverName,
        string ReceiverPhone,
        string ReceiverAddress,
        int OriginCityId,
        int DestinationCityId,
        decimal WeightKg,
        decimal LengthCm,
        decimal WidthCm,
        decimal HeightCm,
        string PackageType,
        string ServiceType,
        decimal Price
    );
}
