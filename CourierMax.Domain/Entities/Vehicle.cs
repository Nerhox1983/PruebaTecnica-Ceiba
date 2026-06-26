namespace CourierMax.Domain.Entities
{
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public decimal MaxWeightKg { get; set; }
        public decimal MaxVolumeM3 { get; set; }
        public bool IsActive { get; set; }
    }
}
