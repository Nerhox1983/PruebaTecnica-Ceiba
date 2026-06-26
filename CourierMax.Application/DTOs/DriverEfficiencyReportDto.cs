namespace CourierMax.Application.Shipments.DTOs
{
    public class DriverEfficiencyReportDto
    {
        public string DriverId { get; set; } = string.Empty;
        public int TotalAssigned { get; set; }
        public int TotalDelivered { get; set; }
        public int TotalCanceled { get; set; }
        public int TotalInTransit { get; set; }
        public double AverageDeliveryTimeDays { get; set; }
        public decimal PercentageWithinSla { get; set; }
        public decimal TotalWeightTransported { get; set; }
    }
}