namespace CourierMax.Application.Shipments.Queries
{
    public record GetDelayedShipmentsQuery(DateTime StartDate, DateTime EndDate);
}
