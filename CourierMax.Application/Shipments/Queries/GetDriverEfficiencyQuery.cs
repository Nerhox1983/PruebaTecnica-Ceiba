namespace CourierMax.Application.Shipments.Queries
{
    public record GetDriverEfficiencyQuery(string DriverId, DateTime StartDate, DateTime EndDate);
}
